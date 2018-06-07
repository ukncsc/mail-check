# UK NCSC Mail Check App

## Introduction

This web application is intended to be used with UKNCSC Mail Check services.

## Background

We initially built an Angular 2 web application in order to provide some basic user access to Mail Check. After some consideration, we changed direction and began building a React application and started to migrate our old code to React components. We are still on a journey with this migration and some of our legacy Angular 2 code is still used in production - this can be found under `<project-directory>/src/angular`.

We have tried several approaches for our React components/containers and our redux store. For example, we used Higher-order Components in our Admin features which we have since decided was a bad idea - we favour using render-props. The `domain-security` directory contains components demonstrate how we want to proceed in terms of testing and coding patterns and should be used as an exemplar.

## Tools

The project was bootstraped with [create-react-app](https://www.npmjs.com/package/create-react-app) and uses [react-scripts](https://www.npmjs.com/package/react-scripts) for build and test scripts - we want to take advantage of the future developments of react-scripts and won't be "ejecting". For testing, we use [react-testing-library](https://www.npmjs.com/package/react-testing-library) to render components and [Jest](https://www.npmjs.com/package/jest) as our test runner - we aim to at least snapshot test every component. Our code is looked after by [ESLint](https://www.npmjs.com/package/eslint) and [Prettier](https://www.npmjs.com/package/prettier).

## Development

### Prerequisites

- [Node.js](https://nodejs.org/en/) version 8 or higher
- [Yarn](https://yarnpkg.com/lang/en/) (NPM can be used, but we use Yarn on the development team and examples here will be using Yarn)

### Configuration

The app requires a `.env` file in the top level of the solution with the necessary config, prefixed with `REACT_APP`. This will then be available in the application under `process.env`. A `NODE_PATH` attribute is required for module resolution.

```sh
NODE_PATH=src/

REACT_APP_API_ENDPOINT=http://localhost:5000 # the base url of the Mail Check API endpoint
```

The `ukncsc-semantic-ui-theme` must be built and linked locally in order to build this app. We have plans to ship this package to NPM in the near future.

```sh
cd <project-directory>/src/react/ukncsc-semantic-ui-theme
yarn build
yarn link
cd <project-directory>/src/react/ukncsc-mail-check-app
yarn link ukncsc-semantic-ui-theme
```

### Scripts

| Command         | Description                                                        |
| --------------- | ------------------------------------------------------------------ |
| `yarn start`    | Serves the app from a local development server with hot reloading. |
| `yarn build`    | Creates a production build of the app with minified static assets. |
| `yarn format`   | Format all of the code.                                            |
| `yarn lint`     | Run the ESLint rules against the code.                             |
| `yarn lint:fix` | Run the ESLint rules against the code and fix simple issues.       |
| `yarn test`     | Run all unit tests.                                                |

## Feature To Do List

- [x] DMARC, SPF and TLS status
- [ ] DKIM status
- [ ] DMARC set up wizard
- [ ] Public email security checker
- [ ] Forensic Report analysis
- [ ] Automated domain ownership

## Technical To Do List

- [x] Test and refactor Domain Security components to be an example of how we want our features to be built and tested
- [ ] Replace redux-thunk with redux-observable to manage state from API calls as RxJS streams
- [ ] Replace and remove legacy Angular 2 code
- [ ] Fix linting issues (and remove rule overrides in `.eslintrc.json`).
- [ ] Remove unused or unnecessary dependencies to reduce build size
- [ ] Agree upon a strategy for testing containers
- [ ] Test and refactor Admin components to remove Higher-order Component logic and to have a less confusing redux store
