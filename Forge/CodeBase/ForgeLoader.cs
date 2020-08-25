#if !UNITY_WSA
using System.Net;
//using System.Security.Cryptography.X509Certificates;
//using System.Net.Security;
using System.Diagnostics;
#endif
using System.Runtime.CompilerServices;
//using System.Drawing; // Not supported in Unity3D
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using Photon;
using Photon.Pun;

namespace Autodesk.Forge.ARKit {

	[System.Serializable]
	public class ProcessedNodesEvent : UnityEvent<float> {
	}

	[System.Serializable]
	public class ProcessingNodesCompletedEvent : UnityEvent<int> {
	}

	public class ForgeLoader : MonoBehaviour {

		#region Fields & Properties
		[Tooltip ("Model URN.")]
		[LabelOverride ("Model URN")]
		public string URN ="" ; // Tooltip does not support { get; set; }

		[Tooltip ("Access Token.")]
		[LabelOverride ("Access Token")]
		public string BEARER ="" ;
		public string _BEARER { 
			get { return (BEARER) ; }
			set {
				BEARER =value ;
				_loader.BEARER =value ;
				UnityEngine.Debug.Log ("New BEARER: " + value) ;
			}
		}

		[Tooltip ("Scene ID.")]
		[LabelOverride ("Scene ID")]
		public string SCENEID ="crgscene" ;

		[Tooltip ("Load Metadata.")]
		public bool LoadMetadata =true ;

		[Tooltip ("Load Meshes & Materials.")]
		[LabelOverride ("Load Meshes & Materials")]
		public bool LoadMesh =true ;

		[Tooltip("Create Colliders.")]
		public bool CreateColliders =true ;

		[Space (10)]

		[SerializeField]
		public ProcessedNodesEvent ProcessedNodes =new ProcessedNodesEvent () ;

		[SerializeField]
		public ProcessingNodesCompletedEvent ProcessingNodesCompleted =new ProcessingNodesCompletedEvent () ;

		protected ForgeLoaderEngine _loader =new ForgeLoaderEngine () ;

		#endregion

		#region Unity APIs
		// https://docs.unity3d.com/Manual/ExecutionOrder.html

		private PhotonView PV;
		
		[PunRPC]
		public void values(string urn, string acctoken)
        {
			URN = urn;
			BEARER = acctoken;
            //UnityEngine.Debug.Log(print);
        }

        public void send()
        {
			PV = GetComponent<PhotonView>();
			PV.RPC("values", RpcTarget.AllBuffered, URN, BEARER);

		}
       

		public void click () {
			
			_loader.ROOT =gameObject ;
			_loader.URN =this.URN ;
			_loader.BEARER =this.BEARER ;
			_loader.LoadMesh =this.LoadMesh ;
			_loader.CreateCollider =this.CreateColliders ;
			_loader.LoadMetadata =this.LoadMetadata ;
			_loader.SCENEID =this.SCENEID ;
			_loader.PROJECTID =this.SCENEID ;
			_loader.SaveToDisk =false ;
			_loader.ProcessedNodes +=new ForgeLoaderEngine.ProcessedNodesDelegate (ProcessedNodesCB);
			_loader.ProcessingNodesCompleted +=new ForgeLoaderEngine.ProcessingNodesCompletedDelegate (ProcessingNodesCompletedCB);		
			_loader.Awake () ;
			click1(); 
		}

		public void click1 () { // Start is only ever called once for a given script
			_loader.Start () ;
		}
		
		protected void Update () {
			_loader.Update () ;
		}

		#endregion

		#region Progress callbacks
		protected void ProcessedNodesCB (ForgeLoaderEngine sender, float pct) {
			if ( ProcessedNodes != null )
				ProcessedNodes.Invoke (pct / 100.0f) ;
		}

		protected void ProcessingNodesCompletedCB (ForgeLoaderEngine sender, int unprocessedObjects) {
			if ( ProcessingNodesCompleted != null )
				ProcessingNodesCompleted.Invoke (unprocessedObjects) ;
			_loader.Sleep () ;
		}

		#endregion

		#region Methods
		protected void Load (string urn, string sceneID, string bearer, bool loadMetadata =true, bool loadMesh =true, bool createColliders =false) {
			gameObject.SetActive (false) ;
			URN =urn ;
			BEARER =bearer ;
			SCENEID =sceneID ;
			LoadMesh =loadMesh ;
			CreateColliders =createColliders ;
			LoadMetadata =loadMetadata ;
			gameObject.SetActive (true) ;
		}

		public static ForgeLoader AddLoaderToGameObject (GameObject root, string urn, string sceneID, string bearer, bool loadMetadata =true, bool loadMesh =true, bool createColliders =false) {
			if ( root == null )
				root =new GameObject (ForgeConstants.ROOT) ;
			root.SetActive (false) ;
			ForgeLoader loader =root.AddComponent<ForgeLoader> () ;
			loader.URN =urn ;
			loader.BEARER =bearer;
			loader.SCENEID =sceneID ;
			loader.LoadMesh =loadMesh ;
			loader.CreateColliders =createColliders ;
			loader.LoadMetadata =loadMetadata ;
			root.SetActive (true) ;
			return (loader) ;
		}

		#endregion

		#region Debug utility
		[MethodImpl (MethodImplOptions.NoInlining)]
		public static string GetCurrentMethod () {
#if UNITY_WSA
			return ("") ;
#else
			StackTrace st =new StackTrace () ;
			StackFrame sf =st.GetFrame (1) ;
			return (sf.GetMethod ().Name) ;
#endif
		}

		#endregion

	}

}
