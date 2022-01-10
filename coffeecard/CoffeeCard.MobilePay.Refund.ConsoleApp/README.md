# Readme

Simple ConsoleApp for refunding completed MobilePay orders.

## Dependencies

The console app depends on `MobilePaySettings` from the ASP.NET `appsettings.json` file and a MobilePay certificate for
communicating with the MobilePay Appswitch API.

## How to run

1. Insert OrderIds in the `input.txt` file, sepereated by a comma
2. Run the conesole app
3. Results will be stored in the `output.txt` file showing which OrderIds were successfully or failed getting refunded