using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Rendering.Isosurface {

	public partial class MetaballCameraConfiguration {
		public Camera camera {get; private set;}
		
		//public CullingGroup cullingGroup {get; private set;}

		//public HashSet<MetaballCharge> visibleCharges {get; private set;}
		//public Vector4[] visibleChargeData {get; private set;}

		private Vector4[] _cameraFrustumCorners;
		public Vector3 sampleResolution = new Vector3(10,10,100);

		public MetaballCameraConfiguration (Camera camera) {
			this.camera = camera;

			//visibleCharges = new HashSet<MetaballCharge>();
			_cameraFrustumCorners = new Vector4[4];

			var systemSettings = MetaballSystem.instance.settings;
			sampleResolution = new Vector3(
				Mathf.RoundToInt(camera.pixelWidth / systemSettings.trianglePixelSize),
				Mathf.RoundToInt(camera.pixelHeight / systemSettings.trianglePixelSize),
				systemSettings.depthSlices
			);

			//cullingGroup = new CullingGroup();
			//cullingGroup.targetCamera = camera;
			//cullingGroup.onStateChanged = OnChargeCullingGroupStateChanged;

			//camera.AddCommandBuffer (CameraEvent.BeforeImageEffects, vertexPrepass);
		}

		public void Dispose () {
			//if (camera) {
			//	camera.RemoveCommandBuffer (CameraEvent.BeforeImageEffects, vertexPrepass);
			//}

			//cullingGroup.Dispose();
			//cullingGroup = null;

			//visibleCharges.Clear();

			if (destroyTemporaryResources != null)
				destroyTemporaryResources();
		}

		/*
		public void OnChargeCullingGroupStateChanged (CullingGroupEvent cullingEvent) {
			if (cullingEvent.index < 0 || cullingEvent.index >= MetaballSystem.instance.charges.Count) 
				return;

			var charge = MetaballSystem.instance.charges[cullingEvent.index];

			if (cullingEvent.hasBecomeInvisible) {
				visibleCharges.Remove(charge);
			}

			if (cullingEvent.hasBecomeVisible) {
				visibleCharges.Add(charge);
			}
		}
		*/

		public Vector4[] GetFrustumCorners () {
			var systemSettings = MetaballSystem.instance.settings;
			float maxDist = Mathf.Min(systemSettings.clipDistance, camera.farClipPlane);

			_cameraFrustumCorners[0] = camera.ViewportToWorldPoint(new Vector3(0, 0, maxDist)); // bottom left
			_cameraFrustumCorners[1] = camera.ViewportToWorldPoint(new Vector3(0, 1, maxDist)); // top left
			_cameraFrustumCorners[2] = camera.ViewportToWorldPoint(new Vector3(1, 1, maxDist)); // top right
			_cameraFrustumCorners[3] = camera.ViewportToWorldPoint(new Vector3(1, 0, maxDist)); // bottom right

			return _cameraFrustumCorners;
		}

	}

} // namespace