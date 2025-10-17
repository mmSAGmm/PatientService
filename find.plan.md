<!-- f16f0ab0-c9ac-4518-b748-5ee860843589 02b37147-24b4-4cba-bb9d-ac89f74cc792 -->
# Find asynchronous methods lacking the Async suffix

### Scope

- Only scan `src/`.
- Treat methods returning `Task`/`Task<T>` or `ValueTask`/`ValueTask<T>` as asynchronous, regardless of `async` modifier presence.

### Approach

1. Search `src/` for method declarations returning `Task`/`ValueTask`.

- Regex examples (language-aware but approximate):
 - `\b(Task|Task<[^>]+>|ValueTask|ValueTask<[^>]+>)\s+([A-Za-z_][A-Za-z0-9_]*)\s*\(`

2. Filter out methods whose names end with `Async` (case-sensitive).
3. Review false positives (fields, properties, local functions) by checking context lines to ensure they are method declarations.
4. Produce a report with:

- File path, line number
- One-line signature
- Suggested corrected name (original + `Async`)

### Deliverables

- A concise list of all offending methods in `src/`, grouped by file, with code references to their signatures.
- Optional follow-up plan: batch rename methods and update call sites across the solution.

### To-dos

- [ ] Search src/ for Task/ValueTask method declarations
- [ ] Filter out methods whose names already end with Async
- [ ] Manually validate candidates to avoid fields/properties/local funcs
- [ ] Output grouped list with signatures and suggested Async names


