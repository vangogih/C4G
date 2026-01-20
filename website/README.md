# C4G Documentation Website

This website is built using [Docusaurus](https://docusaurus.io/), a modern static website generator. The design is inspired by the [Jest website](https://jestjs.io) with C4G's custom theme.

## Features

- 🎨 Jest-inspired design with custom green theme
- 🌍 Full internationalization support (English & Russian)
- 📱 Responsive design with dark/light mode
- 📚 Comprehensive documentation (Installation, OAuth, Google Sheets, Editor Workflow)
- 🚀 Fast static site generation
- 📖 Complete setup guides with step-by-step instructions

## Installation

```bash
npm install
```

## Local Development

```bash
npm start
```

This command starts a local development server at `http://localhost:3000` and opens up a browser window. Most changes are reflected live without having to restart the server.

To start with a specific locale:

```bash
npm start -- --locale ru  # Start with Russian
```

## Build

```bash
npm run build
```

This command generates static content into the `build` directory for all configured locales (English and Russian). The build can be served using any static contents hosting service.

## Serve Built Site

```bash
npm run serve
```

Test the production build locally before deployment.

## Deployment

The site is configured for GitHub Pages deployment:

```bash
GIT_USER=vangogih npm run deploy
```

This command builds the website for all locales and pushes to the `gh-pages` branch.

## Project Structure

```
website/
├── docs/                      # English documentation
│   ├── getting-started.md     # Quick start guide
│   ├── installation.md        # Detailed installation
│   ├── guides/
│   │   ├── overview.md
│   │   ├── oauth-setup.md           # Google OAuth 2.0 setup
│   │   ├── google-sheets-setup.md   # Sheet structure guide
│   │   ├── editor-workflow.md       # Using C4G in Unity
│   │   └── supported-versions.md    # Compatibility info
│   ├── api/
│   │   └── introduction.md
│   ├── adr/                   # Architecture Decision Records
│   └── help.md
├── i18n/
│   └── ru/                    # Russian translations
│       ├── code.json          # UI translations
│       └── docusaurus-plugin-content-docs/
│           └── current/       # Translated docs
├── src/
│   ├── components/
│   │   └── HomepageFeatures/  # Feature cards
│   ├── css/
│   │   └── custom.css         # Jest-inspired theme
│   └── pages/
│       └── index.tsx          # Homepage
├── static/                    # Static assets
└── docusaurus.config.ts       # Main configuration

```

## Configuration

Key configuration files:
- `docusaurus.config.ts` - Main configuration with i18n setup
- `sidebars.ts` - Documentation sidebar structure
- `src/css/custom.css` - Custom theme colors and styles

## Theme Customization

The site uses a Jest-inspired green color scheme:
- Primary color: `#15c213` (light mode)
- Primary color: `#18d316` (dark mode)

## Documentation Structure

### Main Guides

The website includes comprehensive guides for:

1. **Installation** - System requirements, Unity package setup, .NET package setup
2. **OAuth 2.0 Setup** - Step-by-step Google Cloud Console configuration
3. **Google Sheets Setup** - Sheet structure, supported types, best practices
4. **Editor Workflow** - Complete Unity Editor usage guide
5. **Supported Versions** - Unity, .NET, and dependency compatibility

### Contributing to Documentation

When adding new documentation:
1. Create the English version in `docs/`
2. Create the Russian translation in `i18n/ru/docusaurus-plugin-content-docs/current/`
3. Update `sidebars.ts` if adding new sections
4. Test the build: `npm run build`
5. Verify both English and Russian builds succeed

## Useful Commands

```bash
npm run docusaurus        # Access Docusaurus CLI
npm run clear            # Clear cached data
npm run typecheck        # Run TypeScript type checking
npm run write-translations  # Generate translation files
```
