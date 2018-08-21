using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Isosurface {

	[ExecuteInEditMode]
	public class MetaballCharge : MonoBehaviour {

		public float radius = 1;
		public Vector3 position {
			get {
				return transform.position;
			}
		}
		
		public Vector4 packedRepresentation {
			get {
				return new Vector4(position.x, position.y, position.z, radius);
			}
		}

		public BoundingSphere boundingSphere {
			get {
				// multiply the radius by two, because spheres influence geometry which
				// may be visible.
				return new BoundingSphere( position, radius * 2 );
			}
		}

		private void OnEnable () {
			if (MetaballSystem.instance != null) {
				MetaballSystem.instance.RegisterCharge(this);
			}
		}

		private void OnDisable () {
			if (MetaballSystem.instance != null) {
				MetaballSystem.instance.UnregisterCharge(this);
			}
		}

	}

}