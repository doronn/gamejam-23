using System;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;
using UnityEngine.Rendering;

namespace RootsOfTheGods.Scripts.Collectibles
{
    [Serializable]
    public class CollectiblesData
    {
        [field: SerializeField]
        public SerializedCollectiblesDictionary Collectibles { get; private set; }
    }

    [Serializable]
    public class CollectibleData
    {
        [field: SerializeField]
        public string Name { get; private set; }
        
        [field: SerializeField, TextArea]
        public string TextToComment { get; private set; }
    }
    
    [Serializable]
    public class SerializedCollectiblesDictionary : SerializableDictionaryBase<CollectibleType, CollectibleData>
    {
        
    }
}