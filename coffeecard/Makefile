# Forward targets to the Makefile in the repository root.
#
# GNU make only reads a Makefile from the current directory, so this lets
# contributors run `make build`, `make test`, `make format` and
# `make format-check` from the solution directory without changing the
# entry point.

.DEFAULT_GOAL := forward

.PHONY: forward build test format format-check

forward:
	@$(MAKE) -C ..

build test format format-check:
	@$(MAKE) -C .. $@
