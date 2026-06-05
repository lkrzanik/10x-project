# E2E Testing Rules

- Use getByRole, getByLabel, and getByText as primary locators.
- Use getByTestId only when accessibility locators are ambiguous.
- Never use CSS selectors, XPath, or DOM-structure locators.
- Keep tests independently runnable with their own setup and assertions.
- Never use page.waitForTimeout(); wait for state with web-first assertions.
- Assert business outcomes tied to risk, not implementation details.
- Use unique identifiers for mutable test data when writing to persistent storage.
- Prefer storageState authentication for non-authentication risks.
- For authentication risks (R7), UI login in the spec is allowed and preferred.
