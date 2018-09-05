using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer
{
    class Camera
    {
        public Camera()
        {
            m_rotation = new CameraRot();
        }

        public Vector GetPositon()
        {
            return m_position;
        }

        public void SetPositon(Vector pos)
        {
            m_position = pos;
        }

        public void SetRotation()
        {
            Vector pos = new Vector(m_position);
            m_rotation.CameraZ = pos.Normalize();
            //CameraX = CameraZ.CrossProduct(new Vector(0.0f,0.0f,1.0f)).Normalize();
            m_rotation.CameraX = m_rotation.CameraZ.CrossProduct(new Vector(0.0f, 0.0f, 1.0f)).Normalize();
            //m_rotation.CameraX = new Vector(0.0f, 0.0f, 1.0f).CrossProduct(m_rotation.CameraZ).Normalize();
            m_rotation.CameraY = m_rotation.CameraZ.CrossProduct(m_rotation.CameraX).Normalize();
        }

        public class CameraRot {
            public CameraRot()
            {

            }
            public Vector CameraZ;
            public Vector CameraX;
            public Vector CameraY;
        }
        

        private Vector m_position;
        public CameraRot m_rotation;
        private float m_fov;

        public float GetFov (){ return m_fov; }
        public void SetFov(float val) { m_fov = val; }
    }
}
