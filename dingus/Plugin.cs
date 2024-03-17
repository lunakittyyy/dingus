using BepInEx;
using DevHoldableEngine;
using dingus.Behaviors;
using GorillaLocomotion.Swimming;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace dingus
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static AssetBundle bundle;
        public static GameObject dingus;

        void Start() => Utilla.Events.GameInitialized += Init;

        private void Init(object sender, EventArgs e)
        {
            bundle = LoadAssetBundle("dingus.Resources.dingus");
            dingus = Instantiate(bundle.LoadAsset<GameObject>("dingus"));
            DontDestroyOnLoad(dingus);
            
            dingus.transform.position = new Vector3(-66.4f, 14.5f, -82.5f);

            var holdable = dingus.AddComponent<DevHoldable>();
            holdable.Rigidbody = dingus.GetComponent<Rigidbody>();

            holdable.PickUp = true;
            
            dingus.AddComponent<DingusInjury>();
            dingus.AddComponent<RigidbodyWaterInteraction>();

            dingus.layer = 8;
        }

        public AssetBundle LoadAssetBundle(string path)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            AssetBundle bundle = AssetBundle.LoadFromStream(stream);
            stream.Close();
            return bundle;
        }
    }
}
