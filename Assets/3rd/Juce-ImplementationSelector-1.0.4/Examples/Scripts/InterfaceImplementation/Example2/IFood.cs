namespace Juce.ImplementationSelector.Example2
{
    // 下面这个就是把选择的类名字去掉Food字段
    // 比如AppleFood类，选择项机会变成Apple
    //[SelectImplementationTrimDisplayName("Food")]
    public interface IFood {


        void Print();
    }
}
