using System;
using UnityEngine;
using Zenject;


namespace Zenject.MyTest {
    public class TestInstaller : MonoInstaller {
        public override void InstallBindings() {

            XooInstaller.Install(Container);
        }

        #region NonLazy

        private void TestLazy() {
            Debug.LogError("Lazy");
            Container.Bind<IBar>().To<Bar>().AsSingle().OnInstantiated<IBar>(OnInstantiatedFoo).Lazy();
            Container.Bind<Too>().AsSingle();
            Debug.Log(Container.Resolve<Too>());
            Debug.Log(Container.Resolve<IBar>());
        }

        private void TestNonLazy() {
            Debug.LogError("NonLazy");
            Container.Bind<IBar>().To<Bar>().AsSingle().OnInstantiated<IBar>(OnInstantiatedFoo).NonLazy();
            Container.Bind<Too>().AsSingle();
            Debug.Log(Container.Resolve<Too>());
            Debug.Log(Container.Resolve<IBar>());
        }

        private void OnInstantiatedFoo(InjectContext arg1, IBar bar) {
            Debug.Log("Bar ��ʵ����ǰ����");
        }


        #endregion

        private void TestBindInstance() {
            Container.BindInstance("abc");
            Debug.Log(Container.Resolve<string>());
        }

        private void TestFromComponent() {
            var prefabPath = "TestMonoPrefab";
            // ��Ԥ����ȡTestMono
            Container.Bind<TestMono>().FromComponentInNewPrefabResource(prefabPath).AsTransient();
            // ʵ����Ԥ����Ȼ�����
            //Container.Bind<TestMono>().FromNewComponentOnNewPrefabResource(prefabPath).AsTransient();

            for (int i = 0; i < 10; i++) {
                var testmono = Container.Resolve<TestMono>();
                testmono.name = $"TestMono_{i}";
            }
        }

        private void TestFromResolveGetter() {
            Container.Bind<Too>().AsSingle();
            Container.Bind<Bar>().FromResolveGetter<Too>(too => too.GetBar());
            var bar = Container.Resolve<Bar>();
            Debug.Log(bar.name);
        }
    }

    public class Too {
        public override string ToString() {
            return "is Too";
        }

        public Bar GetBar() {
            return new Bar() {
                name = "Too Create Bar"
            };
        }

    }


    public class Foo {
        IBar _bar;

        public Foo(IBar bar) {
            _bar = bar;
        }

        public override string ToString() {
            return _bar == null ? "bar null" : "bar not null";
        }
    }

    public interface IBar {

    }

    public class Bar : IBar {

        public string name;

        public override string ToString() {
            return "is Bar";
        }
    }

    public class XooInstaller : Installer<XooInstaller> {
        public override void InstallBindings() {
            Debug.Log("Xoo InstallBindings");
            //Container.Bind<IInitializable>().To<InterfaceTest>().AsCached();
            //Container.Bind<ITickable>().To<InterfaceTest>().AsCached();
            //Container.Bind<ILateTickable>().To<InterfaceTest>().AsCached();
            //Container.Bind<IFixedTickable>().To<InterfaceTest>().AsCached();
            //Container.Bind<IDisposable>().To<InterfaceTest>().AsCached();
            //Container.Resolve<ITickable>();

            // ������ȼ�
            //Container.Bind(
            //    typeof(InterfaceTest),
            //    typeof(IInitializable),
            //    typeof(ITickable),
            //    typeof(ILateTickable),
            //    typeof(IFixedTickable),
            //    typeof(IDisposable)).To<InterfaceTest>().AsSingle();
            //Container.Resolve<InterfaceTest>();

            // ������ȼ� BindInterfacesAndSelfTo
            // �˽ӿڿ���������ɾ��InterfaceTest��Ľӿ�
            //Container.BindInterfacesAndSelfTo<InterfaceTest>().AsSingle();
            //Container.Resolve<InterfaceTest>();

            // BindInterfacesTo
            //Container.BindInterfacesTo<InterfaceTest>().AsSingle();
            //Container.Resolve<InterfaceTest>(); // ���� InterfaceTestδ��
            //Container.Resolve<IInitializable>(); // ���� �޷�ȡ����

        }
    }

    public class InterfaceTest : IInitializable,ITickable, ILateTickable, IFixedTickable, IDisposable {

        public void Initialize() {
            Debug.Log($"InterfaceTest Initialize");
        }
        public void FixedTick() {
            Debug.Log($"InterfaceTest FixedTick");
        }

        public void LateTick() {
            Debug.Log($"InterfaceTest LateTick");
        }

        public void Tick() {
            Debug.Log($"InterfaceTest Tick");
        }

        public void Dispose() {
            Debug.Log($"InterfaceTest Dispose");
        }
    }
}