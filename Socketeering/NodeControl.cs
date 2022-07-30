using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering
{
    /// <summary>
    /// Node control types
    /// Info/Response:  1*
    /// Requests:       2*      // These are what should be handled in recv functions
    /// Services:       3*      // Optional services a node may advertise
    /// Custom Services:4*-5*   // Custom services
    /// Errors:         9*
    /// </summary>
    public enum NodeControl
    {
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
        /// Responder MUST NOT echo args ID or REF
        /// </summary>
        ECHO = 205,
        /// <summary>
        /// Notify of a node-specific service that is available
        /// MUST send a different message for each available service
        /// MUST notify when a service becomes newly available
        /// SHOULD periodically notify therafter
        /// For standard services (3*)
        ///     MUST set arg - MESSAGE:::[type of message]
        ///     MAY set arg - ARGS:::[required arg types and descriptions]
        ///     MAY set arg - DESCRIPTION:::[description of service]
        ///     MAY set arg - VERSION:::[version of the advertised service]
        /// For non-standard services (4*/5*):
        ///     MUST set arg - DESCRIPTION:::[description of service]
        ///     MUST set arg - MESSAGE:::[new type of message to use and its number]
        ///     MUST set arg - ARGS:::[required arg types and descriptions]
        ///     MUST set arg - VERSION:::[version of the advertised service]
        /// </summary>
        SERVICE = 300,
        /// <summary>
        /// Request for a node to store some data for a given amount of time
        /// Nodes using and advertising this service MUST support REF
        /// MUST set arg - OP:::[GET/SET]
        ///     MUST set arg - PATH:::[path to data], eg logs/temperature/12011995-12:30:22
        ///     Responder MUST respond with INFO
        ///         MUST set arg - VALUE:::[value or empty for missing value]
        /// For GET
        /// FOR SET
        ///     MUST set arg - VALUE:::[value to store]
        ///     MUST set arg - PATH:::[path to data], eg logs/temperature/12011995-12:30:22
        ///         SHOULD be relative path, other node decides root
        ///         MUST NOT contain characters not allowed in a file path
        ///     MAY set arg - UNTIL:::[when the data may be deleted]
        ///         If unset, default is forever
        ///         Formats: 1y2m3d5h2s, MEM (until node closes)
        ///     Responder must explicitly confirm or deny the addition
        ///         For deny:
        ///             MUST use DENIAL/101
        ///             MUST set arg REF:::[request ref]
        ///             MUST set arg REASON::[reason]
        ///                 - Invalid Path
        ///                 - Data size
        ///                 - UNTIL too long/too short
        ///                 - Unathorised
        ///             MAY set arg - REASONDESCRIPTION:::[Human-readable reason]
        ///         For confirm:
        ///             MUST use INFO/100
        ///             MUST set arg - STORED:::[path]
        ///             MUST set arg - UNTIL:::[until]
        ///                 UNTIL MAY or MAY NOT match requested UNTIL but MUST be at least as long as the request, otherwise MUST deny
        /// </summary>
        STORE = 301,
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
        NAME_CLASH = 901,
        /// <summary>
        /// MUST NOT send. Used to identify malformed messages
        /// </summary>
        INVALID_CONTROL = 9999
    }

    public static class NodeControlExt
    {
        public static NodeControl ToNodeControl(this string nodeControl)
        {
            NodeControl parsed;
            if(Enum.TryParse<NodeControl>(nodeControl, out parsed))
            {
                return parsed;
            }

            parsed = NodeControl.INVALID_CONTROL;
            return parsed;
        }
    }
}
