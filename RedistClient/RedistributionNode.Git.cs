using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;

namespace RedistServ
{
    internal partial class RedistributionNode
    {
        private void GitClone(IPAddress ip)
        {
            Trace("Cloning from git");
            Directory.CreateDirectory(_basefolder); //create if root folder not exists
            var url = $"clone http://{ip}:3000/service/{_role.Repository}";
            var branch = string.IsNullOrEmpty(_role.Branch) ? "master" : _role.Branch;
            var targetpath = RepositoryPath();
            var cloneoptions = new CloneOptions()
            {
                BranchName = branch,
                CredentialsProvider = LoginCredentials,
                OnProgress = GitProgressMessageHandler,
                FetchOptions = DefaultFetchOptions,
                OnCheckoutProgress = CheckoutProgressHandler,
                Checkout = true,
            };
            Repository.Clone(url, targetpath, cloneoptions);
                //var command = $"clone http://{ip}:3000/service/{_role.Repository}  --depth 1 --branch {branch}";

            RunSpecialCommand(RepositoryPath(), "lfs install");
            RunSpecialCommand(RepositoryPath(), "lfs fetch");
            RunSpecialCommand(RepositoryPath(), "lfs pull");
            Trace("Cloning finished");
        }

        private FetchOptions DefaultFetchOptions=>new FetchOptions()
        {
            OnProgress = GitProgressMessageHandler,
            CredentialsProvider = LoginCredentials,
            Prune = true,
        };
        private MergeOptions DefaultMergeOptions=>new MergeOptions()
        {
            FileConflictStrategy = CheckoutFileConflictStrategy.Theirs,
            OnCheckoutProgress = CheckoutProgressHandler,
        };

        private readonly CredentialsHandler LoginCredentials = (_url, _user, _cred) =>
            new UsernamePasswordCredentials {Username = "service", Password = "service"};

        private void CheckoutProgressHandler(string path, int completedSteps, int totalSteps)
        {
            Trace($"{completedSteps}/{totalSteps} {path}");
        }

        private void GitPull()
        {
            Trace("Pulling current branch");
            using (var repo = new Repository(RepositoryPath()))
            {
                // Credential information to fetch
                var options = new LibGit2Sharp.PullOptions
                {
                    FetchOptions = DefaultFetchOptions,
                    MergeOptions = DefaultMergeOptions,
                };
                // User information to create a merge commit
                var signature = new LibGit2Sharp.Signature(new Identity("service", "service@ststc.com"), DateTimeOffset.Now);
                // Pull
                Commands.Pull(repo, signature, options);
            }
            RunSpecialCommand(RepositoryPath(), "lfs pull");
        }

        private void GitHardReset()
        {
            using (var repo = new Repository(RepositoryPath()))
            {
                var options=new CheckoutOptions()
                {
                    CheckoutModifiers= CheckoutModifiers.Force,
                    OnCheckoutProgress = CheckoutProgressHandler,
                };
                var currentCommit = repo.Head.Tip;
                repo.Reset(ResetMode.Hard, currentCommit,options);
            }
        }

        private bool GitProgressMessageHandler(string message)
        {
            Trace(message);
            return true;
        }

        private void RunSpecialCommand(string workdir, string command)
        {
            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo("git", command)
                {
                    WorkingDirectory = workdir,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                    //WindowStyle = ProcessWindowStyle.Hidden
                };
                process.OutputDataReceived += (s, e) => Trace(e.Data);
                process.ErrorDataReceived += (s, e) => Error(e.Data);

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
        }


        /*
         * public void FetchAll()
    {
        using (var repo = new Repository(_LocalGitPath))
        {
            foreach (Remote remote in repo.Network.Remotes)
            {
                FetchOptions options = new FetchOptions();
                options.CredentialsProvider = new CredentialsHandler((url, usernameFromUrl, types) => new UsernamePasswordCredentials()
                {
                    Username = _UserName,
                    Password = _Password
                });
                repo.Network.Fetch(remote, options);
            }
        }
    }
    public string CheckoutBranch(string branchName)
    {
        using (var repo = new Repository(_LocalGitPath))
        {
            var trackingBranch = repo.Branches[branchName];

            if (trackingBranch.IsRemote)
            {
                branchName = branchName.Replace("origin/", string.Empty);

                var branch = repo.CreateBranch(branchName, trackingBranch.Tip);
                repo.Branches.Update(branch, b => b.TrackedBranch = trackingBranch.CanonicalName);
                Commands.Checkout(repo, branch, new CheckoutOptions { CheckoutModifiers = CheckoutModifiers.Force });
            }
            else
            {
                Commands.Checkout(repo, trackingBranch, new CheckoutOptions { CheckoutModifiers = CheckoutModifiers.Force });
            }

            return branchName;
        }
    }
    public void PullBranch(string branchName)
    {
        using (var repo = new Repository(_LocalGitPath))
        {
            PullOptions options = new PullOptions();

            options.MergeOptions = new MergeOptions();
            options.MergeOptions.FailOnConflict = true;

            options.FetchOptions = new FetchOptions();
            options.FetchOptions.CredentialsProvider = new CredentialsHandler((url, usernameFromUrl, types) => new UsernamePasswordCredentials()
            {
                Username = _UserName,
                Password = _Password
            });

            repo.Network.Pull(new Signature(_UserName, _Password, new DateTimeOffset(DateTime.Now)), options);
        }
    }
         */
    }
}