using UnityEngine;

public abstract class AbstractSingletonBehaviour<TypeBase, TypeImpl> : MonoBehaviour where TypeBase : MonoBehaviour where TypeImpl : TypeBase {
	private static TypeBase instance = null;
	public static TypeBase Instance {
		get {
			if (instance == null) {
				instance = new GameObject().AddComponent<TypeImpl>();
				var singletonBehaviour = instance as AbstractSingletonBehaviour<TypeBase, TypeImpl>;
				if (singletonBehaviour != null) {
					instance.gameObject.name = singletonBehaviour.SingletonName;
				}
			}
			return instance;
		}
	}

	public AbstractSingletonBehaviour() {}

	public virtual string SingletonName {
		get {
			return GetType().ToString();
		}
	}
}
