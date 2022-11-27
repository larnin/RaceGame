using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NRand
{
    class SimpleQuaternionDistribution : IRandomDistribution<Quaternion>
    {
        UniformFloatDistribution m_distrib = new UniformFloatDistribution(0, 1);

        public Quaternion Max()
        {
            return Quaternion.identity;
        }

        public Quaternion Min()
        {
            return Quaternion.identity;
        }

        public Quaternion Next(IRandomGenerator generator)
        {
            float x = m_distrib.Next(generator);
            float y = m_distrib.Next(generator);
            float z = m_distrib.Next(generator);
            float w = m_distrib.Next(generator);

            var q = new Quaternion(x, y, z, w);
            return q.normalized;
        }
    }
}
