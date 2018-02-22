using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class SimulationRunner
{
    private static SimulationRunner _instance;
    public static void CreateInstance()
    {
        if (_instance == null)
        {
            _instance = new SimulationRunner();
        }
    }
    public static SimulationRunner GetInstance()
    {
        return _instance;
    }

    private SimulationRunner() { }
    
    float timeSeconds = 0;
    float recalculationNeededTime = 0;
    
    void UpdateMachines(List<Machine> machines, float dt)
    {
        foreach (Machine m in machines)
        {
            m._itemsInStorage += m._itemsPerSecondToStorage * dt;
            if (m._itemsInStorage <= 0.001)
                m._itemsInStorage = 0;
        }
    }

    public void Update(List<Machine> machines)
    {
        float desiredDt = 1 / 60.0f;

        while (desiredDt > 0)
        {
            if (timeSeconds + desiredDt > recalculationNeededTime)
            {
                float possibleDt = recalculationNeededTime - timeSeconds;
                UpdateMachines(machines, possibleDt);
                desiredDt -= possibleDt;
                timeSeconds += possibleDt;
                MachineComputations.UpdateRates(machines);
                _instance.recalculationNeededTime = _instance.timeSeconds + machines.Min(x => x.GetSecondsBeforeRecalculationNeeded());
            }
            else
            {
                UpdateMachines(machines, desiredDt);
                timeSeconds += desiredDt;
                break;
            }
        }
    }
}
