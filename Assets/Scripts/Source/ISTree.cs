using System.Collections.Generic;
using UnityEngine;
using ImageSources;
using System;

namespace Source
{
    public class ISTree : MonoBehaviour
    {
        // Number of surfaces
        private int _N;

        // Maximum degree of reflections
        private int _R;

        private List<IS> _ISs;


        // Creates a tree of Image Sources
        // N = number of surfaces
        // R = maximum degree of reflections
        public ISTree(int N, int R)
        {
            _N = N;
            _R = R;
        }


        // Given and index x, returns the degree of reflection for that IS
        public int Degree(int x)
        {
            int d = -1;
            int i = 0;

            while (d < x && i <= _R)
            {
                i++;
                d += IntPow(_N, i);
            }

            return i;
        }




        // Calculates f to the power of p as an integer
        private int IntPow(int f, int p)
        {
            int x = 1;

            for(int i = 0 ; i < p ; i++)
                x *= f;

            return x;
        }
    }
}