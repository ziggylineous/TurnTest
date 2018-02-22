#define ENABLE_UPDATE_FUNCTION_CALLBACK
// #define ENABLE_LATEUPDATE_FUNCTION_CALLBACK
// #define ENABLE_FIXEDUPDATE_FUNCTION_CALLBACK

using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class MainThreadCommandQueue : MonoBehaviour
{
    private static MainThreadCommandQueue instance = null;

    private List<Action> updateCommandQueue = new List<Action>();
	private List<Action> updateCommandQueueCopy = new List<Action>();

	// Used to know if whe have new Action function to execute. This prevents the use of the lock keyword every frame
	private volatile static bool updateCommandsExist = false;

    /*
    private static List<System.Action> actionQueuesLateUpdateFunc = new List<Action>();
    List<System.Action> actionCopiedQueueLateUpdateFunc = new List<System.Action>();
    private volatile static bool noActionQueueToExecuteLateUpdateFunc = true;

    private static List<System.Action> actionQueuesFixedUpdateFunc = new List<Action>();
    List<System.Action> actionCopiedQueueFixedUpdateFunc = new List<System.Action>();
    private volatile static bool noActionQueueToExecuteFixedUpdateFunc = true;
    */

    
    //Used to initialize UnityThread. Call once before any function here
    public static MainThreadCommandQueue Instance
    {
        get {	
			return instance;
		}
    }
    public void Awake()
    {
        Debug.Assert(instance == null);
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

#if (ENABLE_UPDATE_FUNCTION_CALLBACK)
    public void ExecuteCoroutine(IEnumerator action)
    {
        ExecuteInUpdate(() => instance.StartCoroutine(action));
    }

    public void ExecuteInUpdate(Action action)
    {
        Debug.Assert(action != null);

        lock (updateCommandQueue)
        {
            updateCommandQueue.Add(action);
            updateCommandsExist = true;
        }
    }

    public void Update()
    {
        if (updateCommandsExist)
        {
            lock (updateCommandQueue)
			{
				updateCommandQueueCopy.Clear();
				updateCommandsExist = false;
                List<Action> originalCommandQueue = updateCommandQueue;
                updateCommandQueue = updateCommandQueueCopy;
                updateCommandQueueCopy = originalCommandQueue;
			}        
			
            for (int i = 0; i < updateCommandQueueCopy.Count; i++)
				updateCommandQueueCopy[i].Invoke();
        }
    }
#endif

    /*
#if (ENABLE_LATEUPDATE_FUNCTION_CALLBACK)
    public static void executeInLateUpdate(System.Action action)
    {
        if (action == null)
        {
            throw new ArgumentNullException("action");
        }

        lock (actionQueuesLateUpdateFunc)
        {
            actionQueuesLateUpdateFunc.Add(action);
            noActionQueueToExecuteLateUpdateFunc = false;
        }
    }

    public void LateUpdate()
    {
        if (noActionQueueToExecuteLateUpdateFunc)
        {
            return;
        }

        //Clear the old actions from the actionCopiedQueueLateUpdateFunc queue
        actionCopiedQueueLateUpdateFunc.Clear();
        lock (actionQueuesLateUpdateFunc)
        {
            //Copy actionQueuesLateUpdateFunc to the actionCopiedQueueLateUpdateFunc variable
            actionCopiedQueueLateUpdateFunc.AddRange(actionQueuesLateUpdateFunc);
            //Now clear the actionQueuesLateUpdateFunc since we've done copying it
            actionQueuesLateUpdateFunc.Clear();
            noActionQueueToExecuteLateUpdateFunc = true;
        }

        // Loop and execute the functions from the actionCopiedQueueLateUpdateFunc
        for (int i = 0; i < actionCopiedQueueLateUpdateFunc.Count; i++)
        {
            actionCopiedQueueLateUpdateFunc[i].Invoke();
        }
    }
#endif

    ////////////////////////////////////////////FIXEDUPDATE IMPL//////////////////////////////////////////////////
#if (ENABLE_FIXEDUPDATE_FUNCTION_CALLBACK)
    public static void executeInFixedUpdate(System.Action action)
    {
        if (action == null)
        {
            throw new ArgumentNullException("action");
        }

        lock (actionQueuesFixedUpdateFunc)
        {
            actionQueuesFixedUpdateFunc.Add(action);
            noActionQueueToExecuteFixedUpdateFunc = false;
        }
    }

    public void FixedUpdate()
    {
        if (noActionQueueToExecuteFixedUpdateFunc)
        {
            return;
        }

        //Clear the old actions from the actionCopiedQueueFixedUpdateFunc queue
        actionCopiedQueueFixedUpdateFunc.Clear();
        lock (actionQueuesFixedUpdateFunc)
        {
            //Copy actionQueuesFixedUpdateFunc to the actionCopiedQueueFixedUpdateFunc variable
            actionCopiedQueueFixedUpdateFunc.AddRange(actionQueuesFixedUpdateFunc);
            //Now clear the actionQueuesFixedUpdateFunc since we've done copying it
            actionQueuesFixedUpdateFunc.Clear();
            noActionQueueToExecuteFixedUpdateFunc = true;
        }

        // Loop and execute the functions from the actionCopiedQueueFixedUpdateFunc
        for (int i = 0; i < actionCopiedQueueFixedUpdateFunc.Count; i++)
        {
            actionCopiedQueueFixedUpdateFunc[i].Invoke();
        }
    }
#endif
*/

    public void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
}