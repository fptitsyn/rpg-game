using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InfinityPBR.Demo
{
    public class DevilDemo : InfinityDemoCharacter
    {
        [Header("Texture Buttons")]
        public Button[] bodyButtons;
        public Button[] armorButtons;

        [Header("Armor")] 
        public Material[] materialsBodyArmor;
        public GameObject[] bodyArmors;
        public Material[] materialsArmArmor;
        public GameObject[] armArmors;
        public Material[] materialsFur;
        public GameObject[] furArmors;
        public Material[] materialsLeatherArmor;
        public GameObject[] leatherArmors;
        public Material[] materialsLegArmor;
        public GameObject[] legArmors;
        public Material[] materialsLoincloth;
        public GameObject loincloth;
        public Material[] materialsTasset;
        public GameObject[] tassetArmors;
        
        public void RandomizeTextures(){
            bodyButtons[Random.Range(0, bodyButtons.Length)].onClick.Invoke();
            armorButtons[Random.Range(0, armorButtons.Length)].onClick.Invoke();
        }

        public void SetArmorTexture(int index)
        {
            foreach (var obj in bodyArmors) SetTexture(obj, materialsBodyArmor[index]);
            foreach (var obj in armArmors) SetTexture(obj, materialsArmArmor[index]);
            foreach (var obj in furArmors) SetTexture(obj, materialsFur[index]);
            foreach (var obj in leatherArmors) SetTexture(obj, materialsLeatherArmor[index]);
            foreach (var obj in legArmors) SetTexture(obj, materialsLegArmor[index]);
            foreach (var obj in tassetArmors) SetTexture(obj, materialsTasset[index]);
            SetTexture(loincloth, materialsLoincloth[index]);
        }

        private void SetTexture(GameObject obj, Material mat) => obj.GetComponent<SkinnedMeshRenderer>().sharedMaterial = mat;
    }
}