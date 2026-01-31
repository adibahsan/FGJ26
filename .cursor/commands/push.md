# Push Command

Create small, focused commits and push them to the remote repository.

## Process

1. Run `git status` and `git diff` to understand all changes
2. Group related changes into small, logical commits
3. For each group:
   - Stage only the related files
   - Write a concise commit message (1 line, focus on "why" not "what")
4. Push all commits to the remote

## Commit Guidelines

- **One concern per commit** - Don't mix unrelated changes
- **Small commits** - Prefer multiple small commits over one large commit
- **Clear messages** - Use imperative mood ("Add feature" not "Added feature")

## Example

If there are changes to Player.cs, Mask.cs, and GameManager.cs:
- If all relate to "mask pickup", make one commit
- If Player.cs has movement changes AND mask changes, split into two commits
