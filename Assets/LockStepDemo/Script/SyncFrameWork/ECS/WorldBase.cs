﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public abstract class WorldBase
{
    #region 属性

    #region 基础属性

    public string name;

    SyncRule m_syncRule;
    public SyncRule SyncRule
    {
        get
        {
            return m_syncRule;
        }

        set
        {
            m_syncRule = value;
        }
    }

    bool m_isStart = false;
    bool isFinish = false;   //游戏结束

    bool m_isClient = false; //是否是在客户端运行
    bool m_isLocal = false;  //是否单机运行

    bool m_isInterFrame = false;  //是否是插值帧

    int m_frameCount = 0;
    public bool IsClient
    {
        get
        {
            return m_isClient;
        }

        set
        {
            m_isClient = value;
        }
    }
    public bool IsStart
    {
        get
        {
            return m_isStart;
        }

        set
        {
            m_isStart = value;
        }
    }
    public bool IsLocal
    {
        get
        {
            return m_isLocal;
        }

        set
        {
            m_isLocal = value;
        }
    }

    public bool IsInterFrame
    {
        get
        {
            return m_isInterFrame;
        }

        set
        {
            m_isInterFrame = value;
        }
    }

    public int FrameCount
    {
        get
        {
            return m_frameCount;
        }

        set
        {
            m_frameCount = value;
        }
    }
    public bool IsFinish
    {
        get
        {
            return isFinish;
        }

        set
        {
            isFinish = value;
        }
    }

    public ComponentTypeBase componentType;
    /// <summary>
    /// 组件实体缓存池
    /// </summary>
    public ComponentPool heapComponentPool;
    #endregion

    #region 对象和系统的集合

    //Stack<EntityBase> m_entitiesPool = new Stack<EntityBase>();  //TODO: 实体对象池

    public ECSGroupManager group = null;
    public List<SystemBase> m_systemList = new List<SystemBase>();                 //世界里所有的System列表

    public Dictionary<int, EntityBase> m_entityDict = new Dictionary<int, EntityBase>(); //世界里所有的entity集合
    public List<EntityBase> m_entityList = new List<EntityBase>();                       //世界里所有的entity列表

    public Dictionary<string, SingletonComponent> m_singleCompDict = new Dictionary<string, SingletonComponent>(); //所有的单例组件集合
    //public SingletonComponent[] singletonComponents = null;

    #endregion

    #region 回滚相关

    bool m_isCertainty = false;
    public bool IsCertainty
    {
        get {

            if(IsLocal)
            {
                return true;
            }

            if(!IsClient)
            {
                return true;
            }

            return m_isCertainty; }
        set
        {
            m_isCertainty = value;
        }
    }

    bool m_isRecalc = false;
    public bool IsRecalc
    {
        get { return m_isRecalc; }
        set { m_isRecalc = value; }
    }

    public List<RecordSystemBase> m_recordList = new List<RecordSystemBase>();           //世界里所有的RecordSystem列表
    public Dictionary<string, RecordSystemBase> m_recordDict = new Dictionary<string, RecordSystemBase>(); //世界里所有的RecordSystem集合

    #endregion

    #region 事件

    public ECSEvent eventSystem = null;

    public event EntityChangedCallBack OnEntityOptimizeCreated;
    public event EntityChangedCallBack OnEntityOptimizeWillBeDestroyed;
    public event EntityChangedCallBack OnEntityOptimizeDestroyed;

    public event EntityChangedCallBack OnEntityCreated;
    public event EntityChangedCallBack OnEntityWillBeDestroyed;
    public event EntityChangedCallBack OnEntityDestroyed;

    public event EntityComponentChangedCallBack OnEntityComponentAdded;
    public event EntityComponentChangedCallBack OnEntityComponentRemoved;
    public event EntityComponentReplaceCallBack OnEntityComponentChange;

    #endregion

    #endregion

    #region 方法

    #region 重载方法
    public virtual Type[] GetSystemTypes()
    {
        return new Type[0];
    }

    public virtual Type[] GetRecordTypes()
    {
        return new Type[0];
    }

    //要回滚的单例组件
    public virtual Type[] GetRecordSingletonTypes()
    {
        return new Type[0];
    }

    #endregion

    #region 生命周期

    public void Init(bool isClient)
    {
        eventSystem = new ECSEvent(this);

        componentType = GetComponentType();
         heapComponentPool = new ComponentPool(componentType.Count(),this);
        //singletonComponents = new SingletonComponent[componentType.Count()];
        //Debug.Log(" componentType: " + componentType.GetType().FullName);
       IsClient = isClient;
        try
        {
            InitSystem();

            if(IsClient)
            {
                InitRecordSystem();
            }

            InitGroup();
            GameStart();
        }
        catch (Exception e)
        {
            Debug.LogError("WorldBase Init Exception:" + e.ToString());
        }
    }

    private ComponentTypeBase GetComponentType()
    {
        Type t = typeof(ComponentTypeBase);
        Assembly assem = Assembly.GetAssembly(t);

        foreach (Type tChild in assem.GetTypes())
        {
            if (tChild.BaseType == t)
            {
                return (ComponentTypeBase)Activator.CreateInstance(tChild);
            }

        }
       return  (ComponentTypeBase)Activator.CreateInstance(t);
    }

    public void Dispose()
    {
        Debug.Log("world dispose " + m_entityList.Count);

        while(m_entityList.Count > 0)
        {
            DestroyEntityAndDispatch(m_entityList[0]);
        }

        //createCache.Clear();

        for (int i = 0; i < destroyCache.Count; i++)
        {
            DispatchEntityWillBeDestroyed(destroyCache[i]);
            DispatchOptimizeDestroy(destroyCache[i]);
        }
        destroyCache.Clear();

        m_entityList.Clear();
        m_entityDict.Clear();

        for (int i = 0; i < m_systemList.Count; i++)
        {
            m_systemList[i].Dispose();
        }

        m_systemList.Clear();

        m_recordList.Clear();
        m_recordDict.Clear();
    }

    void InitSystem()
    {
        Type[] types = GetSystemTypes();

        for (int i = 0; i < types.Length; i++)
        {
            SystemBase tmp = (SystemBase)types[i].Assembly.CreateInstance(types[i].FullName);
            m_systemList.Add(tmp);
            tmp.m_world = this;
            tmp.Init();
        }
    }

    void InitGroup()
    {
        group = new ECSGroupManager(this);
        OnEntityComponentAdded += group.OnEntityComponentChange;
        OnEntityComponentRemoved += group.OnEntityComponentChange;
        //OnEntityComponentChange += (entity, compName, previousComponent, newComponent) =>
        //{
        //    group.OnEntityComponentChange(entity, compName, newComponent);
        //};
    }

    void GameStart()
    {
        for (int i = 0; i < m_systemList.Count; i++)
        {
            m_systemList[i].OnGameStart();
        }
    }

    #endregion

    #region Update

    #region 外部接口

    /// <summary>
    /// 服务器不执行Loop
    /// </summary>
    public void Loop(int deltaTime)
    {
        if (IsStart)
        {
            BeforeUpdate(deltaTime);
            Update(deltaTime);
            //LateUpdate 另行调用
        }
    }

    public void FixedLoop(int deltaTime)
    {
        if (IsStart)
        {
#if UNITY_ANDROID

            //超过5帧不再预测
            ConnectStatusComponent csc = GetSingletonComp<ConnectStatusComponent>();
            if (csc.confirmFrame <= m_frameCount - 4 && IsLocal == false)
            {
                return;
            }
#endif

            //只有客户端才记录过去值
            if (IsClient)
            {
                Record(FrameCount);
            }

            FrameCount++;

            BeforeFixedUpdate(deltaTime);
            FixedUpdate(deltaTime);
            LateFixedUpdate(deltaTime);

            LazyExecuteEntityOperation();

            EndFrame(deltaTime);
        }
        else
        {
            OnlyCallByPause();
        }
    }

    public void OptimisticFixedLoop(int deltaTime) //乐观帧同步
    {
        if (IsStart)
        {
            IsCertainty = true;

            FrameCount++;

            BeforeFixedUpdate(deltaTime);
            FixedUpdate(deltaTime);
            LateFixedUpdate(deltaTime);

            LazyExecuteEntityOperation();

            EndFrame(deltaTime);
        }
        else
        {
            OnlyCallByPause();
        }
    }

    /// <summary>
    /// 调用重演算
    /// </summary>
    public void CallRecalc()
    {
        for (int i = 0; i < m_systemList.Count; i++)
        {
            m_systemList[i].Recalc();
        }
    }

    /// <summary>
    /// 重演算接口
    /// </summary>
    /// <param name="deltaTime"></param>
    public void Recalc(int frame, int deltaTime)
    {
        FrameCount = frame;

        BeforeFixedUpdate(deltaTime);
        FixedUpdate(deltaTime);
        LateFixedUpdate(deltaTime);

        LazyExecuteEntityOperation();

        EndFrame(deltaTime);
    }
#endregion

    #region Update 只在客户端执行
    void BeforeUpdate(int deltaTime)
    {
        for (int i = 0; i < m_systemList.Count; i++)
        {
            m_systemList[i].BeforeUpdate(deltaTime);
        }
    }

    // Update is called once per frame
    void Update(int deltaTime)
    {
        for (int i = 0; i < m_systemList.Count; i++)
        {
            m_systemList[i].Update(deltaTime);
        }
    }

    public void LateUpdate(int deltaTime)
    {
        for (int i = 0; i < m_systemList.Count; i++)
        {
            m_systemList[i].LateUpdate(deltaTime);
        }
    }

#endregion

    #region FixUpdate 前后端以同样频率执行

    void BeforeFixedUpdate(int deltaTime)
    {
        for (int i = 0; i < m_systemList.Count; i++)
        {
            m_systemList[i].BeforeFixedUpdate(deltaTime);
        }
    }

    void FixedUpdate(int deltaTime)
    {
        for (int i = 0; i < m_systemList.Count; i++)
        {
            m_systemList[i].FixedUpdate(deltaTime);
        }
    }

    void LateFixedUpdate(int deltaTime)
    {
        for (int i = 0; i < m_systemList.Count; i++)
        {
            m_systemList[i].LateFixedUpdate(deltaTime);
        }
    }

#endregion

    #region 其他时刻

    void EndFrame(int deltaTime)
    {
        for (int i = 0; i < m_systemList.Count; i++)
        {
            m_systemList[i].EndFrame(deltaTime);
        }
    }

    void OnlyCallByPause()
    {
        for (int i = 0; i < m_systemList.Count; i++)
        {
            m_systemList[i].RunByPause();
        }
    }

#endregion

#endregion

    #region 回滚相关 

    void InitRecordSystem()
    {
        //初始化RecordSystem
        Type[] types = GetRecordTypes();
        for (int i = 0; i < types.Length; i++)
        {
            if (types[i].IsSubclassOf(typeof(MomentComponentBase)))
            {
                Type type = typeof(RecordSystem<>);
                type = type.MakeGenericType(types[i]);

                RecordSystemBase tmp = (RecordSystemBase)Activator.CreateInstance(type); ;
                m_recordList.Add(tmp);
                m_recordDict.Add(types[i].Name, tmp);
                tmp.m_world = this;
                tmp.Init();
            }
            else
            {
                Debug.LogError(types[i].FullName + " not is MomentComponent!");
            }
        }

        types = GetRecordSingletonTypes();
        for (int i = 0; i < types.Length; i++)
        {
            if(types[i].IsSubclassOf(typeof(MomentSingletonComponent)))
            {
                Type type = typeof(RecordSingletonSystem<>);
                type = type.MakeGenericType(types[i]);

                RecordSystemBase tmp2 = (RecordSystemBase)Activator.CreateInstance(type); ;
                m_recordList.Add(tmp2);
                m_recordDict.Add(types[i].Name, tmp2);
                tmp2.m_world = this;
                tmp2.Init();
            }
            else
            {
                Debug.LogError(types[i].FullName + " not is MomentSingletonComponent!");
            }
        }
    }

    public void Record(int frame)
    {
        if (IsLocal)
            return;

        RandomRecord(frame);

        for (int i = 0; i < m_recordList.Count; i++)
        {
            m_recordList[i].Record(frame);
        }
    }

    public void RevertToFrame(int frame)
    {
        RandomRevertToFrame(frame); //随机数回滚
        EntityRevertToFrame(frame); //实体回滚

        for (int i = 0; i < m_recordList.Count; i++)
        {
            m_recordList[i].RevertToFrame(frame);
        }

        FrameCount = frame;
    }

    public void ClearBefore(int frame)
    {
        for (int i = 0; i < m_recordList.Count; i++)
        {
            m_recordList[i].ClearBefore(frame);
        }
    }

    public void ClearAfter(int frame)
    {
        for (int i = 0; i < m_recordList.Count; i++)
        {
            m_recordList[i].ClearAfter(frame);
        }
    }

    public void ClearRecordAt(int frame)
    {
        for (int i = 0; i < m_recordList.Count; i++)
        {
            m_recordList[i].ClearRecordAt(frame);
        }
    }

    public void ClearAll()
    {
        for (int i = 0; i < m_recordList.Count; i++)
        {
            m_recordList[i].ClearAll();
        }
    }

    public RecordSystemBase GetRecordSystemBase(string name)
    {
        if (!m_recordDict.ContainsKey(name))
        {
            throw new Exception("GetRecordSystemBase error not find " + name);
        }

        return m_recordDict[name];
    }

    public bool GetExistRecordSystem(string name)
    {
        return m_recordDict.ContainsKey(name);
    }

#region 实体回滚

    void RecordEntityCreate(EntityBase entity)
    {
        if (!IsClient)
            return;

        //Debug.Log("EntityRecordSystem OnEntityCreate！ " + entity.ID + " m_isCertainty " + m_isCertainty);

        //只记录预测时的操作
        if (IsCertainty)
        {
            return;
        }

        //Debug.Log(" 记录创建 ID: " + entity.ID + " frame " + entity.m_CreateFrame);

        EntityRecordComponent erc = GetSingletonComp<EntityRecordComponent>();

        //如果此帧有这个ID的摧毁记录，把它抵消掉
        if (erc.GetReordIsExist(entity.CreateFrame, entity.ID,/* systemName, timing,*/ EntityChangeType.Destroy))
        {
            EntityRecordInfo record = erc.GetReord(entity.CreateFrame, entity.ID, /*systemName, timing,*/ EntityChangeType.Destroy);
            //Debug.Log("抵消掉摧毁记录 " + entity.ID);
            erc.m_list.Remove(record);
        }
        else
        {
            EntityRecordInfo info = new EntityRecordInfo();
            info.changeType = EntityChangeType.Create;
            info.id = entity.ID;
            info.frame = entity.CreateFrame;
            //info.systemName = systemName;
            //info.timing = timing;
            info.SaveComp(entity);

            erc.m_list.Add(info);
        }
    }

    void RecordEntityDestroy(EntityBase entity)
    {
        if (!IsClient)
            return;
        //Debug.Log("EntityRecordSystem OnEntityDestroy！ " + entity.ID + " m_isCertainty " + m_isCertainty);

        //只记录预测时的操作
        if (IsCertainty)
        {
            return;
        }

        //Debug.Log(" 记录摧毁 ID: " + entity.ID + " frame " + entity.m_DestroyFrame);

        EntityRecordComponent erc = GetSingletonComp<EntityRecordComponent>();

        //如果此帧有这个ID的创建记录，把它抵消掉
        if (erc.GetReordIsExist(entity.DestroyFrame, entity.ID,  EntityChangeType.Create))
        {
            //Debug.Log("抵消掉创建记录 " + entity.ID);
            EntityRecordInfo record = erc.GetReord(entity.DestroyFrame, entity.ID,EntityChangeType.Create);
            erc.m_list.Remove(record);
        }
        else
        {
            EntityRecordInfo info = new EntityRecordInfo();
            info.changeType = EntityChangeType.Destroy;
            info.id = entity.ID;
            info.frame = entity.DestroyFrame;
            //info.systemName = systemName;
            //info.timing = timing;
            info.SaveComp(entity);

            erc.m_list.Add(info);
        }
    }

    void EntityRevertToFrame(int frame/*,string systemName,RecordEntityTiming timing*/)
    {
        ////Debug.Log("RevertToFrame m_world.Frame " + m_world.FrameCount + " frame " + frame);

        //逐帧倒放
        for (int i = FrameCount; i >= frame + 1; i--)
        {
            EntityRevertOneFrame(i/*, systemName, timing*/);
        }
    }

    void EntityRevertDestroyToFrame(int frame)
    {
        ////Debug.Log("RevertToFrame m_world.Frame " + m_world.FrameCount + " frame " + frame);

        //逐帧倒放
        for (int i = FrameCount; i >= frame + 1; i--)
        {
            EntityRevertDestroyOneFrame(i);
        }
    }

    void EntityRevertOneFrame(int frame/*, string systemName, RecordEntityTiming timing*/)
    {
        //Debug.Log("回退到 " + frame);

        EntityRecordComponent erc = GetSingletonComp<EntityRecordComponent>();

        for (int i = 0; i < erc.m_list.Count; i++)
        {
            if (erc.m_list[i].frame == frame
                //&& erc.m_list[i].systemName == systemName
                // && erc.m_list[i].timing == timing
                  //&& erc.m_list[i].changeType == EntityChangeType.Create
                )
            {
                EntityRevertOneFrame(erc.m_list[i]);

                erc.m_list.RemoveAt(i);
                i--;
            }
        }

        //Debug.Log("回退结束");
    }

    void EntityRevertDestroyOneFrame(int frame)
    {
        //Debug.Log("回退到 " + frame);

        EntityRecordComponent erc = GetSingletonComp<EntityRecordComponent>();

        for (int i = 0; i < erc.m_list.Count; i++)
        {
            if (erc.m_list[i].frame == frame
                  && erc.m_list[i].changeType == EntityChangeType.Destroy
                )
            {
                EntityRevertOneFrame(erc.m_list[i]);

                erc.m_list.RemoveAt(i);
                i--;
            }
        }

        //Debug.Log("回退结束");
    }

    void EntityRevertOneFrame(EntityRecordInfo data)
    {
        if (data.changeType == EntityChangeType.Create)
        {
            //Debug.Log("RevertRecord DestroyEntityNoDispatch " + data.id + " frame " + data.frame + " worldFrame " + FrameCount);
            RollbackCreateEntity(data.id, data.frame);
        }
        else
        {
            //Debug.Log("RevertRecord CreateEntityNoDispatch " + data.id + " frame " + data.frame + " worldFrame" + FrameCount);
            RollbackDestroyEntity(data.id, data.frame, data.compList.ToArray());
        }
    }

    void RollbackCreateEntity(int ID, int frame)
    {
        EntityBase entity = GetEntity(ID);

        entity.DestroyFrame = frame;

        DestroyEntityNoDispatch(entity);

        AddDestroyCache(entity);
    }

    /// <summary>
    /// 回滚摧毁一个实体，不派发事件，并将回滚对象存入缓存
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="compList"></param>
    /// <returns></returns>
    EntityBase RollbackDestroyEntity(int ID, int frame, params ComponentBase[] compList)
    {
        EntityBase entity = NewEntity("RollbackDestroyEntity", ID, compList);

        entity.CreateFrame = frame;

        CreateEntityNoDispatch(entity);

        AddCreateCache(entity);

        return entity;
    }


#endregion

#region 组件回滚



#endregion

#endregion

    #region 实体相关

    //List<EntityBase> createCache = new List<EntityBase>();
    List<EntityBase> destroyCache = new List<EntityBase>();

    //集中执行实体的创建删除操作
    public void LazyExecuteEntityOperation()
    {
        //for (int i = 0; i < createCache.Count; i++)
        //{
        //    AddEntity(createCache[i]);
        //}
        //createCache.Clear();

        for (int i = 0; i < destroyCache.Count; i++)
        {
            RemoveEntity(destroyCache[i]);
        }
        destroyCache.Clear();
    }

#region 创建

    public EntityBase CreateEntity(string identifier, params ComponentBase[] comps)
    {
        identifier = FrameCount + identifier;
        int EntityID = identifier.ToHash();
        EntityBase entity = CreateEntity(identifier, EntityID, comps);

        //Debug.Log("CreateEntity " + identifier + " id " + EntityID);

        return entity;
    }

    /// <summary>
    /// 使用指定的实体ID创建实体，不建议直接使用
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public EntityBase CreateEntity(string name,int ID, params ComponentBase[] compList)
    {
        if (m_entityDict.ContainsKey(ID))
        {
            throw new Exception("CreateEntity Entity ID has exist ! ->" + ID + "<-");
        }

        SyncDebugSystem.AddDebugMsg("Create " + ID + " " + name);

        EntityBase entity = NewEntity(name,ID, compList);
        AddEntity(entity);

        return entity;
    }

    /// <summary>
    /// 立即创建一个实体，不要在游戏逻辑中使用
    /// </summary>
    public EntityBase CreateEntityImmediately(string identifier, params ComponentBase[] compList)
    {
        identifier = FrameCount + identifier;
        int ID = identifier.ToHash();

        if (m_entityDict.ContainsKey(ID))
        {
            throw new Exception("CreateEntity Exception: Entity ID has exist ! ->" + ID + "<-");
        }

        EntityBase entity = NewEntity(identifier, ID, compList);
        CreateEntityAndDispatch(entity);

        return entity;
    }

    EntityBase NewEntity(string name,int ID, params ComponentBase[] compList)
    {
        EntityBase entity = null;

        if(GetIsExistDispatchDestroyCache(ID))
        {
            entity = GetDispatchDestroyCache(ID);
            CopyValue(compList, entity);
        }
        else
        {
            entity = new EntityBase(this);
            entity.ID = ID;
            entity.name = name;
            entity.CreateFrame = FrameCount;

            if (compList != null)
            {
                for (int i = 0; i < compList.Length; i++)
                {
                    if(compList[i] != null)
                    {
                        entity.AddComp(compList[i].GetType().Name, compList[i]);
                    }
                    else
                    {
                        Debug.LogError("NewEntity component is null ! name " + name + " id " + ID);
                    }
                }
            }
        }



        return entity;
    }

    void AddEntity(EntityBase entity)
    {
        if(IsRecalc)
        {
            RecalcCreateEntity(entity);
        }
        else
        {
            //Debug.Log("预测创建 " + entity.ID + " frame " + FrameCount + " m_isCertainty " + m_isCertainty);
            CreateEntityAndDispatch(entity);
            RecordEntityCreate(entity);
        }
    }

    void CreateEntityAndDispatch(EntityBase entity)
    {
        CreateEntityNoDispatch(entity);
        DispatchOptimizeCreate(entity);
    }



    void CreateEntityNoDispatch(EntityBase entity)
    {
        //Debug.Log("从实体列表中添加 " + entity.ID + " frame " + FrameCount);

        if (m_entityDict.ContainsKey(entity.ID))
        {
            Debug.LogError("CreateEntityNoDispatch 创建实体 id 冲突！ " + entity.ID);
        }

        m_entityList.Add(entity);
        m_entityDict.Add(entity.ID, entity);

        entity.OnComponentAdded += DispatchEntityComponentAdded;
        entity.OnComponentRemoved += DispatchEntityComponentRemoved;
        entity.OnComponentReplaced += DispatchEntityComponentChange;

        group.OnEntityCreate(entity);
        DispatchCreate(entity);
    }

#endregion

#region 摧毁

    public void ClientDestroyEntity(int ID)
    {
        //if (m_isView && m_syncRule == SyncRule.Status)
        //{
        //    //状态同步下本地创建的对象才立即销毁
        //    if (ID > 0)
        //    {
        //        return;
        //    }
        //}

        //if (!m_isCertainty)
        //    return;

        //Debug.Log("ClientDestroyEntity ID " + ID + " frame " + FrameCount);

        DestroyEntity(ID);
    }

    public void DestroyEntity(int ID)
    {
        //Debug.Log("DestroyEntity ID " + ID + " frame " + FrameCount);

        if (m_entityDict.ContainsKey(ID))
        {
            EntityBase entity = m_entityDict[ID];

            entity.DestroyFrame = FrameCount;

            if (!destroyCache.Contains(entity))
                destroyCache.Add(entity);
        }
        else
        {
            ////如果还在创建队列则直接移除
            //for (int i = 0; i < createCache.Count; i++)
            //{
            //    if(createCache[i].ID == ID)
            //    {
            //        createCache.RemoveAt(i);
            //        return;
            //    }
            //}
           
            Debug.LogError("DestroyEntity dont ContainsKey ->" + ID + "<-");
        }
    }

    void RemoveEntity(EntityBase entity)
    {
        if(IsRecalc)
        {
            RecalcDestroyEntity(entity);
        }
        else
        {
            //Debug.Log("预测摧毁 " + entity.ID + " frame " + FrameCount + " m_isCertainty " + m_isCertainty);
            DestroyEntityAndDispatch(entity);
            RecordEntityDestroy(entity);
        }
    }

    void DestroyEntityAndDispatch(EntityBase entity)
    {
        DispatchEntityWillBeDestroyed(entity);
        DestroyEntityNoDispatch(entity);
        DispatchOptimizeDestroy(entity);
    }

    void DestroyEntityNoDispatch(EntityBase entity)
    {
        DispatchWillBeDestroy(entity);

        //Debug.Log("从实体列表中移除 " + entity.ID + " frame " + FrameCount);
        m_entityList.Remove(entity);
        m_entityDict.Remove(entity.ID);
        group.OnEntityDestroy(entity);

        DispatchDestroy(entity);

        entity.OnComponentAdded -= DispatchEntityComponentAdded;
        entity.OnComponentRemoved -= DispatchEntityComponentRemoved;
        entity.OnComponentReplaced -= DispatchEntityComponentChange;
    }

    public void DestroyEntityImmediately(int id)
    {
        EntityBase entity = GetEntity(id);
        DestroyEntityAndDispatch(entity);
    }

#endregion

#region 获取对象

    public int GetEntityID(string identifier)
    {
        identifier = FrameCount + identifier;

        return identifier.ToHash();
    }

    public bool GetEntityIsExist(int ID)
    {
        return m_entityDict.ContainsKey(ID);
    }

    public EntityBase GetEntity(int ID)
    {
        try
        {
            return m_entityDict[ID];
        }
        catch
        {
            throw new Exception("GetEntity Exception: Entity ID has not exist ! ->" + ID + "<-");
        }
    }

    public List<EntityBase> GetEntityList(string[] compNames)
    {
        List<EntityBase> tupleList = new List<EntityBase>();
        for (int i = 0; i < m_entityList.Count; i++)
        {
            if (GetAllExistComp(compNames, m_entityList[i]))
            {
                tupleList.Add(m_entityList[i]);
            }
        }

        return tupleList;
    }

    public bool GetAllExistComp(string[] compNames, EntityBase entity)
    {
        for (int i = 0; i < compNames.Length; i++)
        {
            if (!entity.GetExistComp(compNames[i]))
            {
                return false;
            }
        }
        return true;
    }

#endregion

#region 回滚相关

    List<EntityBase> dispatchDestroyCache = new List<EntityBase>();
    List<EntityBase> dispatchCreateCache = new List<EntityBase>();

    void AddCreateCache(EntityBase entity)
    {
        //Debug.Log("AddCreateCache " + dispatchDestroyCache.Count);

        //去重
        if (dispatchCreateCache.Contains(entity))
        {
            Debug.LogError("已在创建缓存中存在 " + entity.ID);
            return;
        }

        //如果创建列表已经存在这个对象则抵消
        if (dispatchDestroyCache.Contains(entity))
        {
            //Debug.Log("抵消创建 " + entity.ID);
            dispatchDestroyCache.Remove(entity);
        }
        else
        {
            dispatchCreateCache.Add(entity);

            //加入派发队列
            //Debug.Log("加入派发创建缓存 " + entity.ID + " " + dispatchCreateCache.Count);
        }
    }

    void AddDestroyCache(EntityBase entity)
    {
        //Debug.Log("AddDestroyCache " + dispatchCreateCache.Count);

        //去重
        if (dispatchDestroyCache.Contains(entity))
        {
            Debug.LogError("已在摧毁缓存中存在 " + entity.ID);
            return;
        }

        if (dispatchCreateCache.Contains(entity))
        {
            //Debug.Log("抵消摧毁 " + entity.ID);
            dispatchCreateCache.Remove(entity);
        }
        else
        {
            //判断抵消
            //Debug.Log("加入派发摧毁缓存 " + entity.ID);
            dispatchDestroyCache.Add(entity);
        }
    }

    public void EndRecalc()
    {
        //Debug.Log("重计算结束 延迟派发事件");

        for (int i = 0; i < dispatchDestroyCache.Count; i++)
        {
            //Debug.Log("派发摧毁 " + dispatchDestroyCache[i].ID + " frame " + FrameCount);
            DispatchEntityWillBeDestroyed(dispatchDestroyCache[i]);
            DispatchOptimizeDestroy(dispatchDestroyCache[i]);
        }
        dispatchDestroyCache.Clear();

        for (int i = 0; i < dispatchCreateCache.Count; i++)
        {
            //Debug.Log("派发创建 " + dispatchCreateCache[i].ID + " frame " + FrameCount);
            DispatchOptimizeCreate(dispatchCreateCache[i]);
        }
        dispatchCreateCache.Clear();
    }

    void RecalcCreateEntity(EntityBase entity)
    {
        //Debug.Log("重计算 创建 " + entity.ID + " frame " + FrameCount);

        AddCreateCache(entity);

        CreateEntityNoDispatch(entity);

        RecordEntityCreate(entity);

    }

    void RecalcDestroyEntity(EntityBase entity)
    {
        //Debug.Log("重计算 摧毁 " + entity.ID + " frame " + FrameCount);

        AddDestroyCache(entity);

        DestroyEntityNoDispatch(entity);

        RecordEntityDestroy(entity);
    }

    void CopyValue(ComponentBase[] from,EntityBase to)
    {
        for (int i = 0; i < from.Length; i++)
        {
            if(from[i] is MomentComponentBase)
            {
                MomentComponentBase mc = (MomentComponentBase)from[i];

                //MomentComponentBase copy = mc.DeepCopy();
                to.ChangeComp(mc.GetType().ToString(), mc);
            }
        }
    }

    public EntityBase GetDispatchDestroyCache(int ID)
    {
        for (int i = 0; i < dispatchDestroyCache.Count; i++)
        {
            if (dispatchDestroyCache[i].ID == ID)
            {
                return dispatchDestroyCache[i];
            }
        }

        throw new Exception("GetCreateRollbackCache not find " + ID);
    }

    public bool GetIsExistDispatchDestroyCache(int ID)
    {
        for (int i = 0; i < dispatchDestroyCache.Count; i++)
        {
            if (dispatchDestroyCache[i].ID == ID)
            {
                return true;
            }
        }

        return false;
    }

    public EntityBase GetDispatchCreateCache(int ID)
    {
        for (int i = 0; i < dispatchCreateCache.Count; i++)
        {
            if (dispatchCreateCache[i].ID == ID)
            {
                return dispatchCreateCache[i];
            }
        }

        throw new Exception("GetDestroyRollbackCache not find " + ID);
    }

    public bool GetIsExistDispatchCreateCache(int ID)
    {
        for (int i = 0; i < dispatchCreateCache.Count; i++)
        {
            if (dispatchCreateCache[i].ID == ID)
            {
                return true;
            }
        }

        return false;
    }

#endregion

#endregion

    #region 单例组件

    public T GetSingletonComp<T>() where T : SingletonComponent, new()
    {
        Type type = typeof(T);

        string key = typeof(T).Name;

        if (type.IsGenericType)
        {
            key += type.GetGenericArguments()[0].Name;
        }

        SingletonComponent comp = null;

        if (m_singleCompDict.ContainsKey(key))
        {
            comp = m_singleCompDict[key];
        }
        else
        {
            comp = new T();
            comp.Init();
            comp.World = this;
            m_singleCompDict.Add(key, comp);
        }

        return (T)comp;
    }

    public SingletonComponent GetSingletonComp(string compName)
    {
       
        //int index = componentType.GetComponentIndex(compName);
        SingletonComponent comp = null;
        if (m_singleCompDict.ContainsKey(compName))
        {
             comp = m_singleCompDict[compName];
        }
        else
        {
            Type compType = Type.GetType(compName);

            comp = (SingletonComponent)compType.Assembly.CreateInstance(compType.FullName);
            comp.Init();
            comp.World = this;
            //singletonComponents[index] = comp;
            m_singleCompDict.Add(compName, comp);
        }

        return comp;
    }

    public void ChangeSingletonComp<T>(T comp) where T : SingletonComponent, new()
    {
        string compName = typeof(T).Name;
        ChangeSingleComp(compName, comp);
    }

    public void ChangeSingleComp(string compName, SingletonComponent comp)
    {
        if (m_singleCompDict.ContainsKey(compName))
        {
           int index = componentType.GetComponentIndex(compName);
            heapComponentPool.PutObject(index, m_singleCompDict[compName]);
            m_singleCompDict[compName] = comp;
        }
        else
        {
            m_singleCompDict.Add(compName, comp);
        }

        comp.World = this;
    }

#endregion

    #region 事件派发

    void DispatchOptimizeCreate(EntityBase entity)
    {
        //Debug.Log("派发创建 " + entity.ID);

        try
        {
            if (OnEntityOptimizeCreated != null)
            {
                OnEntityOptimizeCreated(entity);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("DispatchCreate " + e.ToString());
        }
    }

    void DispatchCreate(EntityBase entity)
    {
        //Debug.Log("派发创建 " + entity.ID);
        try
        {
            if (OnEntityCreated != null)
            {
                OnEntityCreated(entity);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("DispatchCreate " + e.ToString());
        }
    }
    void DispatchEntityWillBeDestroyed(EntityBase entity)
    {
        try
        {
            if (OnEntityOptimizeWillBeDestroyed != null)
            {
                OnEntityOptimizeWillBeDestroyed(entity);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("DispatchDestroy OnEntityWillBeDestroyed: " + e.ToString());
        }
    }

    void DispatchOptimizeDestroy(EntityBase entity)
    {
        //Debug.Log("派发摧毁 " + entity.ID);

        try
        {
            if (OnEntityOptimizeDestroyed != null)
            {
                OnEntityOptimizeDestroyed(entity);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("DispatchDestroy OnEntityDestroyed: " + e.ToString());
        }
    }

    void DispatchDestroy(EntityBase entity)
    {
        //Debug.Log("派发摧毁 " + entity.ID);

        try
        {
            if (OnEntityDestroyed != null)
            {
                OnEntityDestroyed(entity);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("DispatchDestroy OnEntityDestroyed: " + e.ToString());
        }
    }

    void DispatchWillBeDestroy(EntityBase entity)
    {
        //Debug.Log("派发摧毁 " + entity.ID);

        try
        {
            if (OnEntityWillBeDestroyed != null)
            {
                OnEntityWillBeDestroyed(entity);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("DispatchDestroy OnEntityDestroyed: " + e.ToString());
        }
    }

    void DispatchEntityComponentAdded(EntityBase entity, int compIndex, ComponentBase component)
    {
        if (OnEntityComponentAdded != null)
        {
            OnEntityComponentAdded(entity, compIndex, component);
        }
    }

    void DispatchEntityComponentRemoved(EntityBase entity, int compIndex, ComponentBase component)
    {
        if (OnEntityComponentRemoved != null)
        {
            OnEntityComponentRemoved(entity, compIndex, component);
        }
    }

    void DispatchEntityComponentChange(EntityBase entity, int compIndex, ComponentBase previousComponent, ComponentBase newComponent)
    {
        if (OnEntityComponentChange != null)
        {
            OnEntityComponentChange(entity, compIndex, previousComponent, newComponent);
        }
    }

    public delegate void EntityChangedCallBack(EntityBase entity);

#endregion

    #region 随机数

    public int m_RandomSeed = 0;

    int m_randomA = 9301;
    int m_randomB = 49297;
    int m_randomC = 233280;

    public int GetRandom()
    {
        m_RandomSeed = Math.Abs((m_RandomSeed * m_randomA + m_randomB) % m_randomC);

        if (SyncDebugSystem.isDebug)
        {
            SyncDebugSystem.RecordRandomChange(FrameCount, m_RandomSeed, "");
        }

        return m_RandomSeed;
    }

    /// <summary>
    /// 不包括max
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public int GetRandom(int min,int max)
    {
        int result = GetRandom();

        result = min + result % (max - min);

        return result;
    }

    Dictionary<int, int> m_randomDict = new Dictionary<int, int>();
    void RandomRevertToFrame(int frame)
    {
        if (m_randomDict.ContainsKey(frame))
        {
            m_RandomSeed = m_randomDict[frame];
        }
        else
        {
            Debug.LogError("随机数回滚失败  " + frame);
        }
    }

    void RandomRecord(int frame)
    {
        if(m_randomDict.ContainsKey(frame))
        {
            m_randomDict[frame] = m_RandomSeed;
        }
        else
        {
            m_randomDict.Add(frame, m_RandomSeed);
        }
    }

    #endregion
#if !Server
    #region 乐观帧同步

    public bool GetIsAllMsg()
    {
        List<EntityBase> list = GetPlayerList();
        bool isAllMessage = true;
        
        for (int j = 0; j < list.Count; j++)
        {
            AddRecordComponent(list[j]);
            //PlayerCommandRecordComponent tmp = list[j].GetComp<PlayerCommandRecordComponent>(ComponentType.PlayerCommandRecordComponent);
            //isAllMessage &= tmp.GetAllMessage(FrameCount + 1);
        }

        return isAllMessage;
    }

    public int GetCacheCount()
    {
        int count = 0;
        List<EntityBase> list = GetPlayerList();

        bool isAllMessage = true;
        for (int i = m_frameCount + 1; ; i++)
        {
            if (list.Count == 0)
            {
                break;
            }

            for (int j = 0; j < list.Count; j++)
            {
                AddRecordComponent(list[j]);
                //PlayerCommandRecordComponent tmp = list[j].GetComp<PlayerCommandRecordComponent>(ComponentType.PlayerCommandRecordComponent);
                //isAllMessage &= tmp.GetAllMessage(i);
            }

            if (isAllMessage)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        return count;
    }

    public void AddRecordComponent(EntityBase entity)
    {
        //if (!entity.GetExistComp(ComponentType.PlayerCommandRecordComponent))
        //{
        //    PlayerCommandRecordComponent rc = new PlayerCommandRecordComponent();

        //    //自动添加记录组件
        //    entity.AddComp(rc);
        //}
    }

    private bool isGetHashCode = false;
    private int filterNameHashCode;
    string[] filter = new string[] { "CommandComponent", "RealPlayerComponent" };
    public List<EntityBase> GetPlayerList()
    {
        if (!isGetHashCode)
        {
            isGetHashCode = true;

            filterNameHashCode = group.StringArrayToInt(filter);
        }
        return group.GetEntityByFilter(filterNameHashCode, filter);
    }
    #endregion
#endif
#endregion
}

/// <summary>
/// 同步规则
/// </summary>
public enum SyncRule
{
    /// <summary>
    /// 状态同步，所有对实体的操作都交给服务器下发,服务器可能针对每个玩家做剪枝
    /// </summary>
    Status,

    /// <summary>
    /// 帧同步，本地计算所有结果，本地了解游戏里的一切情况
    /// </summary>
    Frame,
}

