public interface IFlammable {
    bool IsOnFire { get; }
    bool CanCatchFire { get; }

    void Ignite();
    void Extinguish();
}