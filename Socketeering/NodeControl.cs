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
    /// Requests:       2* // These are what should be handled in recv functions
    /// Errors:         9*
    /// </summary>
    public enum NodeControl
    {
        /// <summary>
        /// Message containing a node name
        /// MAY include args of key:val pairs of other information
        /// When used a a response to a request (2*) message:
        ///     MUST include arg - REQUEST:[NodeControl] of original request
        /// </summary>
        INFO = 100,
        /// <summary>
        /// Notification that a request will not be fulfilled
        /// MUST not be used as a response to unimplemented controls
        /// MUST include arg - REQUEST:[NodeControl] of original request
        /// MUST include arg- REASON
        /// </summary>
        DENIAL = 101,
        /// <summary>
        /// Message containing the current time of a node
        /// MUST include arg - TIME:[current time]
        /// </summary>
        TIME_SYNC = 103,
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
        /// Request from one node to another to stop sending messages
        /// </summary>
        CLOSE = 299,
        /// <summary>
        /// Notification that a previous request is not implemented
        /// MUST include the request as args
        /// </summary>
        NOT_IMPLEMENTED = 900,
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
