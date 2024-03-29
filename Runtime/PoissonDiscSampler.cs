﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

namespace Utilikit {
    /// Poisson-disc sampling using Bridson's algorithm.
    /// Adapted from Mike Bostock's Javascript source: http://bl.ocks.org/mbostock/19168c663618b7f07158
    ///
    /// See here for more information about this algorithm:
    ///   http://devmag.org.za/2009/05/03/poisson-disk-sampling/
    ///   http://bl.ocks.org/mbostock/dbb02448b0f93e4c82c3
    ///
    /// Usage:
    ///   PoissonDiscSampler sampler = new PoissonDiscSampler(10, 5, 0.3f);
    ///   foreach (Vector2 sample in sampler.Samples()) {
    ///       // ... do something, like instantiate an object at (sample.x, sample.y) for example:
    ///       Instantiate(someObject, new Vector3(sample.x, 0, sample.y), Quaternion.identity);
    ///   }
    ///
    /// Author: Gregory Schlomoff (gregory.schlomoff@gmail.com)
    /// Released in the public domain
    public class PoissonDiscSampler {
        private const int k = 30;  // Maximum number of attempts before marking a sample as inactive.

        private readonly Rect rect;
        private readonly float minRadius2;  // radius squared
        private readonly float maxRadius2;
        private readonly float a;
        private readonly float cellSize;
        private Vector2?[,] grid;
        private List<Vector2> activeSamples = new List<Vector2>();

        /// Create a sampler with the following parameters:
        ///
        /// width:  each sample's x coordinate will be between [0, width]
        /// height: each sample's y coordinate will be between [0, height]
        /// radius: each sample will be at least `radius` units away from any other sample, and at most 2 * `radius`.
        public PoissonDiscSampler( float width, float height, float minRadius, float maxRadius ) {
            this.rect = new Rect( 0, 0, width, height );
            this.minRadius2 = minRadius * minRadius;
            this.maxRadius2 = maxRadius * maxRadius;
            this.a = 2.0f / ( maxRadius2 - minRadius2 );
            this.cellSize = minRadius / Mathf.Sqrt( 2 );
            this.grid = new Vector2?[Mathf.CeilToInt( width / cellSize ),
                               Mathf.CeilToInt( height / cellSize )];
        }

        public Vector2? Sample( Func<Vector2, bool> filter = null ) {
            if ( activeSamples.Count == 0 ) {
                // first sample, pick random spot
                Vector2 startSample = new Vector2( Random.value * rect.width, Random.value * rect.height );
                if ( filter != null ) {
                    int attempts = k;
                    while ( !filter( startSample ) ) {
                        if ( attempts <= 0 )
                            return null;

                        startSample = new Vector2( Random.value * rect.width, Random.value * rect.height );
                        attempts--;
                    }
                }
                AddSample( startSample );
                return startSample;
            }
            else {
                // Pick a random active sample
                while ( activeSamples.Count > 0 ) {
                    int i = (int) Random.value * activeSamples.Count;
                    Vector2 sample = activeSamples[i];

                    // Try `k` random candidates between [radius, 2 * radius] from that sample.
                    for ( int j = 0; j < k; ++j ) {

                        float angle = 2 * Mathf.PI * Random.value;
                        float
                            r = Mathf.Sqrt( 2 * Random.value / a +
                                            minRadius2 ); // See: http://stackoverflow.com/questions/9048095/create-random-number-within-an-annulus/9048443#9048443
                        Vector2 candidate = sample + r * new Vector2( Mathf.Cos( angle ), Mathf.Sin( angle ) );

                        // Accept candidates if it's inside the rect and farther than 2 * radius to any existing sample.
                        if ( rect.Contains( candidate ) && IsFarEnough( candidate ) &&
                             ( filter == null || filter( candidate ) ) ) {
                            AddSample( candidate );
                            return candidate;
                        }
                    }

                    // If we couldn't find a valid candidate after k attempts, remove this sample from the active samples queue
                    activeSamples[i] = activeSamples[activeSamples.Count - 1];
                    activeSamples.RemoveAt( activeSamples.Count - 1 );
                }
                    
                return null;
            }
        }

        /// Return a lazy sequence of samples. You typically want to call this in a foreach loop, like so:
        ///   foreach (Vector2 sample in sampler.Samples()) { ... }
        public IEnumerable<Vector2> Samples( Func<Vector2, bool> filter = null ) {
            Vector2? next = Sample( filter );
            while ( next.HasValue ) {
                yield return next.Value;
                next = Sample( filter );
            }
        }

        private bool IsFarEnough( Vector2 sample ) {
            GridPos pos = new GridPos( sample, cellSize );

            int xmin = Mathf.Max( pos.x - 2, 0 );
            int ymin = Mathf.Max( pos.y - 2, 0 );
            int xmax = Mathf.Min( pos.x + 2, grid.GetLength( 0 ) - 1 );
            int ymax = Mathf.Min( pos.y + 2, grid.GetLength( 1 ) - 1 );

            for ( int y = ymin; y <= ymax; y++ ) {
                for ( int x = xmin; x <= xmax; x++ ) {
                    Vector2? s = grid[x, y];
                    if ( s.HasValue ) {
                        Vector2 d = s.Value - sample;
                        if ( d.x * d.x + d.y * d.y < minRadius2 ) return false;
                    }
                }
            }

            return true;

            // Note: we use the zero vector to denote an unfilled cell in the grid. This means that if we were
            // to randomly pick (0, 0) as a sample, it would be ignored for the purposes of proximity-testing
            // and we might end up with another sample too close from (0, 0). This is a very minor issue.
        }

        /// Adds the sample to the active samples queue and the grid before returning it
        private void AddSample( Vector2 sample ) {
            activeSamples.Add( sample );
            GridPos pos = new GridPos( sample, cellSize );
            grid[pos.x, pos.y] = sample;
        }

        /// Helper struct to calculate the x and y indices of a sample in the grid
        private struct GridPos {
            public int x;
            public int y;

            public GridPos( Vector2 sample, float cellSize ) {
                x = (int)( sample.x / cellSize );
                y = (int)( sample.y / cellSize );
            }
        }
    }
}
