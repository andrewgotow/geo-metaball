#if UNITY_EDITOR
	using UnityEditor;
	using UnityEditor.SceneManagement;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Rendering.Isosurface {

[System.Serializable]
public class MetaballSystemSettings {
	[Range(10,200)]
	public float trianglePixelSize = 8;
	[Range(10,200)]
	public int depthSlices = 100;
	[Range(10,100)]
	public float clipDistance = 50;
}

[ExecuteInEditMode]
public partial class MetaballSystem : MonoBehaviourSingleton<MetaballSystem> {

	public const int maxCharges = 128;

	public MetaballSystemSettings settings = new MetaballSystemSettings();

	private List<MetaballCharge> _charges = new List<MetaballCharge>();
	private Vector4[] _chargeData = new Vector4[ maxCharges ];
	//private static BoundingSphere[] _chargeBounds = new BoundingSphere[ maxCharges ];

	private Dictionary<Camera, MetaballCameraConfiguration> _cameraConfigs = new Dictionary<Camera, MetaballCameraConfiguration>();
	private HashSet<Camera> _excludedCameras = new HashSet<Camera>();

	private void OnEnable () {
		Camera.onPreCull += OnCameraPreCull;
		ResetSystem();
	}

	private void OnDisable () {
		ResetSystem();
		Camera.onPreCull -= OnCameraPreCull;

		if (destroyTemporaryResources != null) 
			destroyTemporaryResources();
	}

	private void Awake () {
		// locate all charges in the scene, and add them to our queue.
		var foundCharges = Object.FindObjectsOfType(typeof(MetaballCharge));
		foreach (var charge in foundCharges) {
			RegisterCharge(charge as MetaballCharge);
		}
	}

	public void ResetSystem () {
		_charges.Clear();
		
		foreach (var config in _cameraConfigs.Values) {
			config.Dispose();
		}
		_cameraConfigs.Clear();
		_excludedCameras.Clear();
	}

	public void ExcludeCamera (Camera camera) {
		_excludedCameras.Add(camera);
	}

	public void IncludeCamera (Camera camera) {
		_excludedCameras.Remove(camera);
	}

	public void RegisterCharge (MetaballCharge charge) {
		if (!_charges.Contains(charge)) {
			_charges.Add(charge);

			/*
			foreach (var config in _cameraConfigs.Values) {
				config.cullingGroup.SetBoundingSphereCount(charges.Count);
			}
			*/
		}
	}

	public void UnregisterCharge (MetaballCharge charge) {
		_charges.Remove(charge);
		/*
		foreach (var config in _cameraConfigs.Values) {
			config.cullingGroup.SetBoundingSphereCount(charges.Count);
		}
		*/
	}
	
	private void Update () {
		for (int i = 0; i < _charges.Count; i ++) {
			_chargeData[i] = _charges[i].packedRepresentation;
			//_chargeBounds[ index ] = charge.boundingSphere;
		}
	}

	private void OnCameraPreCull ( Camera camera ) {
		if (!camera)
			return;
		
		if (_excludedCameras.Contains(camera))
			return;

		#if UNITY_EDITOR
			// The frame debugger is rendered through a temporary "Camera", and we
			// shouldn't interfere with that. If the camera we're dealing with is
			// used for an editor preview, then ignore it.
			if (camera.cameraType == CameraType.Preview)
				return;
		#endif
		
		MetaballCameraConfiguration cameraConfig = null;
		if (_cameraConfigs.ContainsKey(camera)) {
			cameraConfig = _cameraConfigs[camera];
		}else{
			cameraConfig = _cameraConfigs[camera] = new MetaballCameraConfiguration(camera);
			//config.cullingGroup.SetBoundingSpheres(_chargeBounds);
		}
		
		BindSurfaceProperties();
		BindCameraProperties( cameraConfig );

		// Field pre-pass currently disabled. The additional performance gain isn't worth the
		// hassle of regenerating the commandbuffer on the fly. Plus, the lack of 3D blit support
		// means that each depth slice of the field texture has to be blitted separately.
		// I may be able to unroll the 3D texture into a packed 2D texture down the line.
		//Graphics.ExecuteCommandBuffer( cameraConfig.fieldPrepass );
		Graphics.ExecuteCommandBuffer( cameraConfig.vertexPrepass );
		Graphics.DrawMesh( cameraConfig.sampleMesh, Matrix4x4.identity, isosurfaceMaterial, 0, camera );
	}
	
	private void BindSurfaceProperties () {
		Shader.SetGlobalInt( "_Metaball_NumCharges", _charges.Count );
		Shader.SetGlobalVectorArray( "_Metaball_Charges", _chargeData );
	}

	private void BindCameraProperties ( MetaballCameraConfiguration config ) {
		Shader.SetGlobalVectorArray( "_CameraFrustumCorners", config.GetFrustumCorners() );
		Shader.SetGlobalVector( "_CameraWorldPosition", config.camera.transform.position );
		Shader.SetGlobalVector( "_Isosurface_VoxelResolution", config.sampleResolution );
		Shader.SetGlobalTexture( "_Isosurface_VertexTexture", config.vertexTexture );
	}

}

} // namespace