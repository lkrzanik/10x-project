# Plan: E0-3 — Bazowy layout Razor z nawigacją (PL)

## Goal

Deliver a responsive Bootstrap 5 layout with Polish navigation, consistent page structure (header, content, footer), and integrated `_LoginPartial`. All existing views render within this layout.

## Phases

### Phase 1 — Layout, navigation & footer

**Files:**
- `app/Views/Shared/_Layout.cshtml` — full Bootstrap 5 layout with Polish navbar
- `app/Views/Shared/_LoginPartial.cshtml` — already exists, ensure included
- `app/Views/_ViewStart.cshtml` — ensure references `_Layout`
- `app/Views/_ViewImports.cshtml` — ensure tag helpers registered
- `app/Views/Home/Index.cshtml` — minimal landing page content (Polish)
- `app/wwwroot/css/site.css` — base custom styles (if needed)

**Actions:**
1. Rewrite `_Layout.cshtml`: HTML5 doctype, Bootstrap 5 CDN (CSS + JS bundle), responsive navbar with brand "10xPV", Polish nav links ("Strona główna"), `_LoginPartial` in navbar, `@RenderBody()`, sticky footer with copyright
2. Ensure `_ViewStart.cshtml` sets `Layout = "_Layout"`
3. Ensure `_ViewImports.cshtml` has `@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers`
4. Update `Home/Index.cshtml` with a simple Polish welcome heading
5. Add minimal `site.css` for footer stickiness

**Success:** `dotnet run` → login → Home page renders with navbar (brand + "Strona główna" + user email + "Wyloguj"), content area, and footer. Responsive on mobile.

## Contracts

| Symbol | Type | Location |
|--------|------|----------|
| `_Layout.cshtml` | shared layout | `app/Views/Shared/` |
| `_ViewStart.cshtml` | view config | `app/Views/` |
| `_ViewImports.cshtml` | tag helpers | `app/Views/` |
| `site.css` | stylesheet | `app/wwwroot/css/` |

## Risks

- Bootstrap CDN downtime in offline dev — acceptable for now, can vendor later
- Nav links point to controllers not yet created — only include Home for now

## Progress

<!-- Updated by /10x-implement -->