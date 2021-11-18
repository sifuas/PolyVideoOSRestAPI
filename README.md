# PolyVideoOSRestAPI

Project with the basic communication structure to connect and send commands to the API of Poly devices running their VideoOS platform.

This is not a complete implementation of the API, as it was written to provide control of the Poly Device Mode on the G7500 and X50 units.

The code was most recently tested and is running on systems with Poly G7500 units. Initial testing was done with X50 and *should* work the same on all VideoOS units that support Poly USB Device Mode.

Note that the Poly VideoOS devices seem to lock up and need a reboot if you keep the RESTful connection open to the units. After a period of time the REST connection and web ui will return errors and stop working requiring a power cycle of the unit.

Make sure to add logic into your own code that will properly connect and disconnect the module from the VideoOS device to keep it from locking up.
