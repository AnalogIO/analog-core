# Forward formatting targets to the Makefile in the repository root.
#
# GNU make only reads a Makefile from the current directory, so this lets
# contributors run `make format` and `make format-check` from the solution
# directory without changing the entry point.

.DEFAULT_GOAL := forward

.PHONY: forward format format-check

forward:
	@$(MAKE) -C ..

format format-check:
	@$(MAKE) -C .. $@
