# Formatting targets for the CoffeeCard solution.
#
# The CSharpier version is pinned in .config/dotnet-tools.json and the style is
# defined in .csharpierrc.json (with matching editor settings in .editorconfig).
# These targets are intended to be run from the repository root; coffeecard/
# forwards to them so the commands also work from the solution directory.

ROOT := $(patsubst %/,%,$(dir $(abspath $(lastword $(MAKEFILE_LIST)))))

.DEFAULT_GOAL := help

.PHONY: help format format-check

help: ## Show this help
	@echo "Usage: make <target>"
	@echo
	@echo "  format        Format all C# and project files with CSharpier"
	@echo "  format-check  Check formatting without changing files (used by CI)"

format: ## Format all C# and project files with CSharpier
	@cd "$(ROOT)" && dotnet tool restore
	@cd "$(ROOT)" && dotnet csharpier format .

format-check: ## Check formatting without changing files (used by CI)
	@cd "$(ROOT)" && dotnet tool restore
	@cd "$(ROOT)" && dotnet csharpier check .
