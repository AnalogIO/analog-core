## Contributing

We are inviting everyone to report issues and create pull requests to the repository. In order to align our implementations, we try to follow the principles from this guide: <https://docs.microsoft.com/en-us/aspnet/core/tutorials/web-api-vsc?view=aspnetcore-3.1>.

### Git branch structure

We follow a [Git Flow](https://nvie.com/posts/a-successful-git-branching-model/) inspired setup. The branch structure is as following :

- `develop` **(Main  branch)** The develop branch is the main branches. All new branches must check out from here into feature branches and merged back into develop. The contents of the development branch would usually reflect what is deployed to the test environment.
- `production` The production branch reflects the current deployment in production. The production branch is merged with the develop branch every time a new version is deployed to production.
- `feature/{author}/{feature-name}` New features are developed on feature branches following the *feature / author name / feature name branch* structure.

### Code formatting

C# and project files are formatted with [CSharpier](https://csharpier.com/). The version is pinned in `.config/dotnet-tools.json` and the style is defined in `.editorconfig`. Since `.editorconfig` sets `root = true`, formatting settings from directories above the repository are ignored, so the result is the same no matter where the repository is checked out or from which folder a command is run.

Format the whole repository with:

```bash
make format
```

Verify formatting without writing any changes (the same check that runs in CI):

```bash
make format-check
```

Without `make`, the equivalent commands are run from the repository root:

```bash
dotnet tool restore
dotnet csharpier format .
```

VS Code users should install the [CSharpier - Code Formatter](https://marketplace.visualstudio.com/items?itemName=csharpier.csharpier-vscode) extension. The workspace settings use it as the formatter for C# files, so format-on-save matches the command line.

### Merging with the develop branch

**A pull request must be created and approved before merging with develop!**

We use a **rebase** strategy when merging to develop. When a feature has been finished in development, the feature branch must be rebased with develop before merging in order to avoid ugly merge commits.

A rebase-merge to develop can be done in command line

```bash
git fetch
git checkout feature/author/feature-name # checkout the feature branch
git rebase origin/develop # rebase with remote develop branch
# resolve any conflicts
git checkout develop
git merge feature/author/feature-name
git push
```
