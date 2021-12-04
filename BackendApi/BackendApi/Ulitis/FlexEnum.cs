using System.Reflection;

namespace BackendApi.Ulitis; 

#pragma warning disable CS0660, CS0661
public abstract class FlexEnum<TValue, E> where E : FlexEnum<TValue, E> where TValue: IEquatable<TValue> {

    public uint Id { get; init; }
    public TValue Value { get; init; }
    
    protected FlexEnum(uint id, TValue value) {
        Id = id;
        Value = value;
    }
        
    public sealed override bool Equals(Object? obj) {
        return obj is null
            ? throw new NullReferenceException($"{nameof(obj)} is null")
            : ((FlexEnum<TValue, E>) obj).Id == this.Id;
    }
    
    public static bool operator ==(FlexEnum<TValue, E> x, FlexEnum<TValue, E> y) => x.Id == y.Id;
    public static bool operator !=(FlexEnum<TValue, E> x, FlexEnum<TValue, E> y) => x.Id != y.Id;

    public int GetHashCode(uint obj) => base.GetHashCode();

    public sealed override string ToString() => $"Id: {Id}, Value: {Value} ";

    private readonly Func<E>?[] _properties = typeof(E)
        .GetProperties(BindingFlags.Static | BindingFlags.Public)
        .Select(i => Delegate.CreateDelegate(typeof(Func<E>), i.GetGetMethod()!) as Func<E>).ToArray();
        
    public E? FindInFlexEnum(TValue findValue) {
        foreach (var property in _properties) {
            var value = property!();
            if (!value.Value.Equals(findValue)) continue;
            return (E)value;
        }

        return null;
    }
        
    public E? FindInFlexEnum(TValue findValue, Func<TValue, TValue, bool> func) {
        foreach (var property in _properties) {
            var value = property!();
            if (!func(findValue, value.Value)) continue;
            return value;
        }

        return null;
    }
}