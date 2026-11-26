# Build, test and formatting targets for the CoffeeCard solution.
#
# The SDK version is selected by global.json in the repository root.
# The CSharpier version is pinned in .config/dotnet-tools.json and the style is
# defined in .csharpierrc.json (with matching editor settings in .editorconfig).
# These targets are intended to be run from the repository root; coffeecard/
# forwards to them so the commands also work from the solution directory.

# Translation: the absolute path of the directory containing this Makefile, with a trailing slash removed.
ROOT := $(patsubst %/,%,$(dir $(abspath $(lastword $(MAKEFILE_LIST)))))

.DEFAULT_GOAL := help

.PHONY: help build test format format-check

help: ## Show this help
	@echo "Usage: make <target>"
	@echo
	@echo "  build         Build the CoffeeCard solution"
	@echo "  test          Build the solution and run all tests"
	@echo "  format        Format all C# and project files with CSharpier"
	@echo "  format-check  Check formatting without changing files (used by CI)"

build: ## Build the CoffeeCard solution
	@cd "$(ROOT)" && dotnet build coffeecard/

test: ## Build the solution and run all tests
	@cd "$(ROOT)" && dotnet test coffeecard/

format: ## Format all C# and project files with CSharpier
	@cd "$(ROOT)" && dotnet tool restore
	@cd "$(ROOT)" && dotnet csharpier format .

format-check: ## Check formatting without changing files (used by CI)
	@cd "$(ROOT)" && dotnet tool restore
	@cd "$(ROOT)" && dotnet csharpier check .
