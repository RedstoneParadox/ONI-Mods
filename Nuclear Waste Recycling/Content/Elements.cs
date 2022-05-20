using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NuclearWasteRecycling.Content
{
    class Elements
    {
        public static SimHashes MOXFuel = EnumPatch.RegisterSimHash("MOXFuel");

        public static void RegisterSubstances(Hashtable substanceList)
        {
            substanceList.Add(MOXFuel, CreateSubstance(MOXFuel, "mox_fuel_kanim", Element.State.Solid, Color.green));
        }

        public static Substance CreateSubstance(SimHashes id, string uiAnim, Element.State state, Color color)
        {
            var animFile = Assets.Anims.Find(anim => anim.name == uiAnim);
            var material = GetMaterialForState(state);

            return ModUtil.CreateSubstance(id.ToString(), state, animFile, material, color, color, color);
        }

        private static Material GetMaterialForState(Element.State state)
        {
            // (gases use liquid material)
            var material = state == Element.State.Solid ? Assets.instance.substanceTable.solidMaterial : Assets.instance.substanceTable.liquidMaterial;
            return new Material(material);
        }
        public static void SetSolidMaterials()
        {
            var folder = Path.Combine(Util.ModPath, "assets", "elements", "textures");
            //var shinyMaterial = Assets.instance.substanceTable.GetSubstance(SimHashes.Diamond).material;
            var shinyMaterial = Assets.instance.substanceTable.GetSubstance(SimHashes.Cuprite).material;

            SetTextures(MOXFuel, null, folder, "mox_fuel", "mox_fuel_specular");
        }

        public static Material SetTextures(SimHashes id, Material newMaterial, string folder, string texture, string spec = null)
        {
            var substance = ElementLoader.FindElementByHash(id).substance;
            var tex = FUtility.Assets.LoadTexture(texture, folder);

            if (newMaterial != null)
            {
                substance.material = new Material(newMaterial);
            }

            substance.material.mainTexture = tex;

            if (!spec.IsNullOrWhiteSpace())
            {
                var specTex = FUtility.Assets.LoadTexture(spec, folder);
                substance.material.SetTexture("_ShineMask", specTex);
            }

            return substance.material;
        }
    }
}
