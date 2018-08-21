using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rendering.Isosurface {

public partial class MetaballSystem : MonoBehaviourSingleton<MetaballSystem> {

	internal delegate void DestroyTemporaryResourcesDelegate ();
	private static DestroyTemporaryResourcesDelegate destroyTemporaryResources;

	// Render Materials
	internal static Material _fieldPrepassMaterial;
	public static Material fieldPrepassMaterial {
		get {
			if (_fieldPrepassMaterial == null) {
				var shader = Shader.Find("Hidden/Gotow/Isosurface/Field_Prepass");
				if (shader == null)
					throw new System.Exception("Could not find shader \"Hidden/Gotow/Isosurface/Field_Prepass\". Make sure it's in the \"always include\" list in the graphics settings.");

				_fieldPrepassMaterial = new Material(shader);
				_fieldPrepassMaterial.name = "Gotow - Isosurface Vertex Prepass Material";
				_fieldPrepassMaterial.hideFlags = HideFlags.HideAndDontSave;

				MetaballSystem.destroyTemporaryResources += () => {
					ObjectUtility.SafeDestroy(_fieldPrepassMaterial);
				};
			}
			return _fieldPrepassMaterial;
		}
	}

	internal static Material _vertexPrepassMaterial;
	public static Material vertexPrepassMaterial {
		get {
			if (_vertexPrepassMaterial == null) {
				var shader = Shader.Find("Hidden/Gotow/Isosurface/Vertex_Prepass");
				if (shader == null)
					throw new System.Exception("Could not find shader \"Hidden/Gotow/Isosurface/Vertex_Prepass\". Make sure it's in the \"always include\" list in the graphics settings.");

				_vertexPrepassMaterial = new Material(shader);
				_vertexPrepassMaterial.name = "Gotow - Isosurface Vertex Prepass Material";
				_vertexPrepassMaterial.hideFlags = HideFlags.HideAndDontSave;

				MetaballSystem.destroyTemporaryResources += () => {
					ObjectUtility.SafeDestroy(_vertexPrepassMaterial);
				};
			}
			return _vertexPrepassMaterial;
		}
	}

	internal static Material _isosurfaceMaterial;
	public static Material isosurfaceMaterial {
		get {
			if (_isosurfaceMaterial == null) {
				var shader = Shader.Find("Hidden/Gotow/Isosurface/Isosurface");
				if (shader == null)
					throw new System.Exception("Could not find shader \"Hidden/Gotow/Isosurface/Isosurface\". Make sure it's in the \"always include\" list in the graphics settings.");

				_isosurfaceMaterial = new Material(shader);
				_isosurfaceMaterial.name = "Gotow - Isosurface Material";
				_isosurfaceMaterial.hideFlags = HideFlags.HideAndDontSave;

				MetaballSystem.destroyTemporaryResources += () => {
					ObjectUtility.SafeDestroy(_isosurfaceMaterial);
				};
			}
			return _isosurfaceMaterial;
		}
	}

	internal static Material _isosurfaceDebugMaterial;
	public static Material isosurfaceDebugMaterial {
		get {
			if (_isosurfaceDebugMaterial == null) {
				var shader = Shader.Find("Hidden/Isosurface/Debug_DrawSamples");
				if (shader == null)
					throw new System.Exception("Could not find shader \"Hidden/Isosurface/Debug_DrawSamples\". Make sure it's in the \"always include\" list in the graphics settings.");

				_isosurfaceDebugMaterial = new Material(shader);
				_isosurfaceDebugMaterial.name = "Gotow - Isosurface Debug Material";
				_isosurfaceDebugMaterial.hideFlags = HideFlags.HideAndDontSave;

				MetaballSystem.destroyTemporaryResources += () => {
					ObjectUtility.SafeDestroy(_isosurfaceDebugMaterial);
				};
			}
			return _isosurfaceDebugMaterial;
		}
	}

}

} // namespace