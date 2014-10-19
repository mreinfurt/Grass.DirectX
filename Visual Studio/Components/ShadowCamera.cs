using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Wheat.Components
{
    class ShadowCamera
    {
        #region Fields

        Vector3 _position;

        #endregion

        #region Public Methods

        public ShadowCamera()
        {
            _position = new Vector3(0, 10, 10);
        }

        #endregion
    }
}
