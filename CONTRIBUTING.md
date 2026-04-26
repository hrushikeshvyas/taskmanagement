# Contributing Guidelines

## Code Style

- Use **PascalCase** for class/method names
- Use **camelCase** for local variables
- Use **UPPER_SNAKE_CASE** for constants

## Feature Development

1. **Create vertical slice** in `Application/Features/`
2. **Write handlers** following CQRS pattern (Command/Query → Handler → Response)
3. **Add validation** in handler
4. **Write tests** with mocks
5. **Update controller** with new endpoints

## Naming Conventions
CreateTaskCommand       ← CQRS Command CreateTaskHandler       ← Handler CreateTaskResponse      ← Response DTO
GetByIdQuery           ← CQRS Query GetByIdHandler         ← Handler GetByIdResponse        ← Response DTO
UpdateTaskCommand       ← CQRS Command UpdateTaskHandler       ← Handler UpdateTaskResponse      ← Response DTO
DeleteTaskCommand       ← CQRS Command DeleteTaskHandler       ← Handler DeleteTaskResponse      ← Response DTO

## Testing Requirements

- ✅ Happy path
- ❌ Validation failures
- ❌ Business rule violations
- ❌ Not found scenarios

## Commit Messages
feat: Add task creation feature fix: Resolve validation bug docs: Update README test: Add CreateTaskHandler tests

## Pull Request Process

1. Fork and create feature branch
2. Write tests first (TDD)
3. Implement feature
4. Run full test suite
5. Submit PR with description
