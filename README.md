# Sockets
 Experiments with C# sockets.


# Protocol Spec

- Text-based
- Message look like:
    - `Sam-John-Amrit-Melissa;*|ECHO!VALUE:::"PING"`
    - or generally: `Source;Destination|NodeControl|Key:val,Key:val`
    - NodeControl is the type of message (see below)



## Message details

This is documented in the `NodeControl` enum, but reproduced here with some extra detail.


### Definitions

`MAY`: Nodes can choose to implement this or not but should always accept valid messages even if some information is ignored

`SHOULD`: Nodes can opt not to do this, but it is encouraged. Vice-versa for `SHOULD NOT`.

`MUST`: Nodes must implement this to be spec-compliant. Vice-versa for `MUST NOT`

### Message specs

All messages MAY include an `ID` arg and/or a `REF` arg. `ID`s MUST be GUIDS and `REF`s MUST refer to a previously sent message's `ID` which was addressed to the node.

Message source MUST be set to the sending node's name.

Destination SHOULD be a node name or `*` for a global message to all nodes, unless otherwise limited by the message type.

```C#
/*

    100-199: Informational messages

    200-299: Requests

    900-999: Errors
*/

/// <summary>
/// General informational message
/// MAY include args of key:::val pairs of other information
/// When used a a response to a request (2*) message:::
///     MUST include arg - REQUEST:::[NodeControl] of original request
/// </summary>
INFO = 100,
/// <summary>
/// Notification that a request will not be fulfilled
/// MUST NOT be used as a response to unimplemented controls
/// MUST include arg - REQUEST:::[type of original request]
/// MUST include arg- REASON
/// </summary>
DENIAL = 101,
/// <summary>
/// Message to notify all nodes a node is alive
/// MUST set destination to *
/// MAY send arg - "UPTIME:::[node uptime]"
/// Nodes SHOULD periodically send ALIVE messages
/// Nodes MUST send an initial ALIVE message when they join a network
/// </summary>
ALIVE = 102,
/// <summary>
/// Message containing the current time of a node
/// MUST include arg - TIME:::[current time]
/// MAY include arg - ZONE:::[time-zone]
/// </summary>
TIME_SYNC = 103,
/// <summary>
/// Message containing the old name of a node
/// MUST include arg - OLDNAME:::[old node name]
/// </summary>
NAME_CHANGE = 104,
/// <summary>
/// Message notifying of connectivity to a given remote with a given method
/// MUST set arg - REMOTE:::[remote address]
/// MUST set arg - METHOD:::[method of connection] eg, HTTP
/// MUST set arg - REACHABLE:::[true|false]
/// </summary>
CONNECTIVITY_SYNC = 105,
/// <summary>
/// Message notifying of a node disconnecting from the network
/// MAY set arg - WILLWAIT:::[time to disconnect], eg WILLWAIT:2m1s
///     Nodes MAY send updated DISCONNECTING messages with new WILLWAIT estimates
///     Nodes that set arg WILLWAIT MUST stay connected for at least that long unless they send an updated WILLWAIT estimate
/// Nodes sending DISCONNECTING messages without WILLWAIT arg SHOULD stay connected for a short time after to recieve any final replies
/// Nodes that reply to DISCONNECTING messages SHOULD prioritise replies to these messages based on the WILLWAIT arg
/// </summary>
DISCONNECTING = 106,
/// <summary>
/// Request to send node name
/// </summary>
NAME = 200,
/// <summary>
/// Request for node version
/// </summary>
VERSION = 201,
/// <summary>
/// Request for node time
/// </summary>
TIME = 202,
/// <summary>
/// Request for a node to check connectivity to a remote location
/// MUST include arg - REMOTE:::[remote address]
/// MUST include arg - METHOD:::[method to use]
///     Method to use eg, HTTP, HTTPS, PING
/// Responder MUST respond with CONNECTIVITY_SYNC
/// </summary>
CONNECTIVITY = 203,
/// <summary>
/// Request of a node to forward a message
///     MUST NOT set destination as *
///     Node MAY NOT send if final relay destination is that node
///     Node MAY forward via another node
/// MUST include arg - TO:::[final destination]
/// MUST include arg - MESSAGE:::[message to forward]
///     The forwarding message is any valid string representation of a message eg,
///     Damico-Amrit-Valeri-Merrissa;*|TIME!ZONE:::+0
///     Forwarded message MAY be itself a RELAY message
/// </summary>
RELAY = 204,
/// <summary>
/// Request for node to echo given args
///     MUST NOT set destination as *
/// MAY set arg - VALUE:::[any value]
/// MAY set any args
/// Responder MUST echo VALUE
/// Responder MAY echo other args
/// </summary>
ECHO = 205,
/// <summary>
/// Request from one node to another to stop sending messages
/// </summary>
CLOSE = 299,
/// <summary>
/// Notification that a previous request is not implemented
/// MUST include arg REQUEST:::[original request]
///     Original request MAY be a NodeControl or MAY be a NodeControl;Subset eg, CONNECTIVITY;HTTP
/// </summary>
NOT_IMPLEMENTED = 900,
/// <summary>
/// Notification that a node has determined that more than one node has the same name
///     MUST set destination as *
/// MUST set arg - NAME:::[clashing name]
/// Nodes recieving a NAME_CLASH whose name is clashing must select a new name
/// </summary>
NAME_CLASH = 901
```