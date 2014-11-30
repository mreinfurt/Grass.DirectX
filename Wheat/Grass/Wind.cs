using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace Wheat.Grass
{
    class Wind
    {
        /// <summary>
        /// The current acceleration of the wind
        /// </summary>
        public Vector2 Acceleration { get; set; }

        /// <summary>
        /// The current state of the wind (current velocity)
        /// </summary>
        public Vector2 Velocity { get; set;  }

        public Wind()
        {
            this.Acceleration = new Vector2();
            this.Velocity = new Vector2();
        }

        public Wind(Vector2 acceleration, Vector2 velocity)
        {
            this.Acceleration = acceleration;
            this.Velocity = velocity;
        }
    }
}
