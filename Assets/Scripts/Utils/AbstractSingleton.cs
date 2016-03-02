public abstract class AbstractSingleton<TypeBase, TypeImpl> where TypeBase : class where TypeImpl : TypeBase, new() {
	private static TypeBase instance = null;
	public static TypeBase Instance {
		get {
			if (instance == null) {
				instance = new TypeImpl();
			}
			return instance;
		}
	}

	public AbstractSingleton() {}

	public virtual string SingletonName {
		get {
			return GetType().ToString();
		}
	}
}
