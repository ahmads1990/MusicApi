# Music Api 🎧🎵

ASP.NET Music API is designed to deliver music library metadata access, stream music, and manage user accounts. It can be used to support a frontend Single Page Application (SPA).

## Quick start

work in progress

## Table of Contents

-   [Introduction](#introduction)
-   [Motivation](#motivation)
-   [Technologies](#technologies)
-   [Features](#features)
    -   [Future features](#future-features)
-   [Getting Started](#getting-started)
    -   [Prerequisites](#prerequisites)
    -   [Installation](#installation)
-   [Project walkthrough](#project-walkthrough)
-   [Contributing](#contributing)
    -   [Suggesting ideas](#suggesting-ideas)
    -   [How to Contribute](#how-to-contribute)
    -   [Reporting Bugs](#reporting-bugs)

## Introduction

ASP.NET Music API is designed to deliver music library metadata access using entity framework for database access, stream music, and manage user accounts using identity. It can be used to support a frontend Single Page Application (SPA).
Well tested using Nunit and sqlite provider

## Motivation

Provide quick access to music library metadata and can support SPA

## Technologies

<a href="https://skillicons.dev">
<img src="https://skillicons.dev/icons?i=cs,dotnet,git" />
</a>

Additional used

-   Identity, Entity Framework, MS SQL Server, Nunit

## Features [🔼](#table-of-contents)

-   CRUD song data
-   Support user account

### Future features [🔼](#table-of-contents)

-   CRUD album data
-   Support playlists
-   Streaming tracks
-   Integrating with external services

## Getting Started [🔼](#table-of-contents)

### Prerequisites

    -   Dotnet runtime - 8.0
    -   Visual studio - 22

### Installation

Steps to install and run this project locally

-   Step 1 clone project in terminal paste:

    ```bash
    git clone https://github.com/ahmads1990/MusicApi
    ```

-   Step 2 setup configuration for for database access and token in following this format

    ```json
    {
    	"ConnectionStrings": {
    		"DefaultConnection": "Your connection string"
    	}
    }
    ```

## Project walkthrough [🔼](#table-of-contents)

video or screenshots

## Contributing [🔼](#table-of-contents)

Thank you for considering contributing to our project! Whether it's reporting issues, submitting bug fixes, or proposing new features, your contributions are welcome and appreciated.

### Suggesting ideas [🔼](#table-of-contents)

We welcome suggestions for new features or improvements. Please open an issue to discuss your ideas before starting to work on them or send me a email.

### How to Contribute [🔼](#table-of-contents)

1. Fork the repository to your GitHub account.
2. Checkout installation section
3. Create a new branch for your contribution:

    ```bash
    git checkout -b feature-branch
    ```

4. Make your changes and commit them with a descriptive commit message:

    ```bash
    git commit -m "Add your descriptive message here"
    ```

5. Push the changes to your forked repository:

    ```bash
    git push origin feature-branch
    ```

6. Open a pull request on the original repository. Provide a clear title and description for your pull request, explaining the changes you made.

### Reporting Bugs [🔼](#table-of-contents)

If you find a bug, please open an issue and provide detailed information, including:

-   Steps to reproduce the bug
-   Expected behavior
-   Actual behavior
-   Screenshot (optional)
-   Environment details (e.g., operating system, browser, etc.)

Thank you for contributing!
