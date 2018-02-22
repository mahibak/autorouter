using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass()]
    public class ProductionSpeedComputationTests
    {
        void Reset()
        {
            MachineManager.Reset();
        }

        Machine CreateMachine(int ins, int outs)
        {
            Machine m = new Machine();
            m._inputs = new MachineConnector[ins];
            for (int i = 0; i < m._inputs.Length; i++)
                m._inputs[i] = new MachineConnector();
            m._outputs = new MachineConnector[outs];
            for (int i = 0; i < m._outputs.Length; i++)
                m._outputs[i] = new MachineConnector();
            MachineManager.CreateInstance();
            MachineManager.RegisterMachine(m);
            return m;
        }

        [TestMethod()]
        public void UpdateDesiredProductionSpeedTestFromToStorage()
        {
            Reset();
            List<Machine> machines = new List<Machine>();

            Machine m1 = CreateMachine(0, 1);
            m1._maximumItemsPerSecond = Single.PositiveInfinity;
            m1._storageCapacity = 1000;
            m1._itemsInStorage = 1000;
            m1._storageMode = Machine.StorageModes.Out;

            Machine m2 = CreateMachine(1, 0);
            m2._maximumItemsPerSecond = 5;
            m2._storageCapacity = 1000;
            m2._storageMode = Machine.StorageModes.In;

            MachineManager.ConnectMachines(m1, 0, m2, 0);
            
            Assert.AreEqual(m1._itemsPerSecondToOutputs, 5);
            Assert.AreEqual(m1._itemsPerSecondToStorage, -5);

            Assert.AreEqual(m2._itemsPerSecondToOutputs, 0);
            Assert.AreEqual(m2._itemsPerSecondToStorage, 5);
        }

        [TestMethod()]
        public void UpdateDesiredProductionSpeedTestInputStorageLimited()
        {
            Reset();
            List<Machine> machines = new List<Machine>();

            Machine m1 = CreateMachine(0, 1);
            m1._maximumItemsPerSecond = Single.PositiveInfinity;
            m1._storageCapacity = 1000;
            m1._itemsInStorage = 3;
            m1._storageMode = Machine.StorageModes.Out;

            Machine m2 = CreateMachine(1, 0);
            m2._maximumItemsPerSecond = 5;
            m2._storageCapacity = 1000;
            m2._storageMode = Machine.StorageModes.In;

            MachineManager.ConnectMachines(m1, 0, m2, 0);
            
            Assert.AreEqual(m1._itemsPerSecondToOutputs, 3);
            Assert.AreEqual(m1._itemsPerSecondToStorage, -3);
            
            Assert.AreEqual(m2._itemsPerSecondToOutputs, 0);
            Assert.AreEqual(m2._itemsPerSecondToStorage, 3);
        }

        [TestMethod()]
        public void UpdateDesiredProductionSpeedTestOutputStorageLimited()
        {
            Reset();
            List<Machine> machines = new List<Machine>();

            Machine m1 = CreateMachine(0, 1);
            m1._maximumItemsPerSecond = Single.PositiveInfinity;
            m1._storageCapacity = 1000;
            m1._itemsInStorage = 1000;
            m1._storageMode = Machine.StorageModes.Out;

            Machine m2 = CreateMachine(1, 0);
            m2._maximumItemsPerSecond = 5;
            m2._storageCapacity = 1000;
            m2._itemsInStorage = 997;
            m2._storageMode = Machine.StorageModes.In;

            MachineManager.ConnectMachines(m1, 0, m2, 0);
            
            Assert.AreEqual(m1._itemsPerSecondToOutputs, 3);
            Assert.AreEqual(m1._itemsPerSecondToStorage, -3);
            
            Assert.AreEqual(m2._itemsPerSecondToOutputs, 0);
            Assert.AreEqual(m2._itemsPerSecondToStorage, 3);
        }

        [TestMethod()]
        public void UpdateDesiredProductionSpeedTestMaxFromStorage()
        {
            Reset();
            List<Machine> machines = new List<Machine>();

            Machine m1 = CreateMachine(0, 1);
            m1._maximumItemsPerSecond = Single.PositiveInfinity;
            m1._storageCapacity = 1000;
            m1._itemsInStorage = 1000;
            m1._maximumItemsPerSecond = 3;
            m1._storageMode = Machine.StorageModes.Out;

            Machine m2 = CreateMachine(1, 0);
            m2._maximumItemsPerSecond = 5;
            m2._storageCapacity = 1000;
            m2._storageMode = Machine.StorageModes.In;

            MachineManager.ConnectMachines(m1, 0, m2, 0);
            
            Assert.AreEqual(m1._itemsPerSecondToOutputs, 3);
            Assert.AreEqual(m1._itemsPerSecondToStorage, -3);
            
            Assert.AreEqual(m2._itemsPerSecondToOutputs, 0);
            Assert.AreEqual(m2._itemsPerSecondToStorage, 3);
        }

        [TestMethod()]
        public void UpdateDesiredProductionSpeedTestMaxOutput()
        {
            Reset();
            List<Machine> machines = new List<Machine>();

            Machine m1 = CreateMachine(0, 1);
            m1._maximumItemsPerSecond = Single.PositiveInfinity;
            m1._storageCapacity = 1000;
            m1._itemsInStorage = 1000;
            m1._maximumItemsPerSecond = 3;
            m1._storageMode = Machine.StorageModes.Out;

            Machine m2 = CreateMachine(1, 0);
            m2._maximumItemsPerSecond = 5;
            m2._storageCapacity = 1000;
            m2._storageMode = Machine.StorageModes.In;

            MachineManager.ConnectMachines(m1, 0, m2, 0);
            
            Assert.AreEqual(m1._itemsPerSecondToOutputs, 3);
            Assert.AreEqual(m1._itemsPerSecondToStorage, -3);
            
            Assert.AreEqual(m2._itemsPerSecondToOutputs, 0);
            Assert.AreEqual(m2._itemsPerSecondToStorage, 3);
        }

        [TestMethod()]
        public void UpdateDesiredProductionSpeedTestProducer()
        {
            Reset();
            List<Machine> machines = new List<Machine>();

            Machine m1 = CreateMachine(0, 1);
            m1._maximumItemsPerSecond = 10;

            Machine m2 = CreateMachine(1, 0);
            m2._maximumItemsPerSecond = 5;
            m2._storageCapacity = 1000;
            m2._storageMode = Machine.StorageModes.In;

            MachineManager.ConnectMachines(m1, 0, m2, 0);
            
            Assert.AreEqual(m1._itemsPerSecondToOutputs, 5);
            Assert.AreEqual(m1._itemsPerSecondToStorage, 0);
            
            Assert.AreEqual(m2._itemsPerSecondToOutputs, 0);
            Assert.AreEqual(m2._itemsPerSecondToStorage, 5);
        }

        [TestMethod()]
        public void UpdateDesiredProductionSpeedTestMaxProd()
        {
            Reset();
            List<Machine> machines = new List<Machine>();

            Machine m1 = CreateMachine(0, 1);
            m1._maximumItemsPerSecond = 3;

            Machine m2 = CreateMachine(1, 0);
            m2._maximumItemsPerSecond = 5;
            m2._storageCapacity = 1000;
            m2._storageMode = Machine.StorageModes.In;

            MachineManager.ConnectMachines(m1, 0, m2, 0);
            
            Assert.AreEqual(m1._itemsPerSecondToOutputs, 3);
            Assert.AreEqual(m1._itemsPerSecondToStorage, 0);
            
            Assert.AreEqual(m2._itemsPerSecondToOutputs, 0);
            Assert.AreEqual(m2._itemsPerSecondToStorage, 3);
        }

        [TestMethod()]
        public void UpdateDesiredProductionSpeedTestThreeMachines()
        {
            Reset();
            List<Machine> machines = new List<Machine>();

            Machine m1 = CreateMachine(0, 1);
            m1._maximumItemsPerSecond = 5;

            Machine m2 = CreateMachine(1, 1);
            m2._maximumItemsPerSecond = 5;

            Machine m3 = CreateMachine(1, 0);
            m3._maximumItemsPerSecond = 5;
            m3._storageCapacity = 1000;
            m3._storageMode = Machine.StorageModes.In;

            MachineManager.ConnectMachines(m1, 0, m2, 0);
            MachineManager.ConnectMachines(m2, 0, m3, 0);
            
            Assert.AreEqual(m1._itemsPerSecondToOutputs, 5);
            Assert.AreEqual(m1._itemsPerSecondToStorage, 0);
            
            Assert.AreEqual(m2._itemsPerSecondToOutputs, 5);
            Assert.AreEqual(m2._itemsPerSecondToStorage, 0);
            
            Assert.AreEqual(m3._itemsPerSecondToOutputs, 0);
            Assert.AreEqual(m3._itemsPerSecondToStorage, 5);
        }

        [TestMethod()]
        public void UpdateDesiredProductionSpeedTestThreeMachinesWithMiddleStorage()
        {
            Reset();
            List<Machine> machines = new List<Machine>();

            Machine m1 = CreateMachine(0, 1);
            m1._maximumItemsPerSecond = 5;

            Machine m2 = CreateMachine(1, 1);
            m2._maximumItemsPerSecond = 5;
            m2._storageCapacity = 1000;
            m2._storageMode = Machine.StorageModes.In;

            Machine m3 = CreateMachine(1, 0);
            m3._maximumItemsPerSecond = 4;
            m3._storageCapacity = 1000;
            m3._storageMode = Machine.StorageModes.In;

            MachineManager.ConnectMachines(m1, 0, m2, 0);
            MachineManager.ConnectMachines(m2, 0, m3, 0);
            
            Assert.AreEqual(m1._itemsPerSecondToOutputs, 5);
            Assert.AreEqual(m1._itemsPerSecondToStorage, 0);
            
            Assert.AreEqual(m2._itemsPerSecondToOutputs, 4);
            Assert.AreEqual(m2._itemsPerSecondToStorage, 1);
            
            Assert.AreEqual(m3._itemsPerSecondToOutputs, 0);
            Assert.AreEqual(m3._itemsPerSecondToStorage, 4);
        }

        [TestMethod()]
        public void UpdateDesiredProductionSpeedTestThreeMachinesWithMiddleStorageOutput()
        {
            Reset();
            List<Machine> machines = new List<Machine>();

            Machine m1 = CreateMachine(0, 1);
            m1._maximumItemsPerSecond = 5;

            Machine m2 = CreateMachine(1, 1);
            m2._maximumItemsPerSecond = 3;
            m2._storageCapacity = 1000;
            m2._itemsInStorage = 1000;
            m2._storageMode = Machine.StorageModes.Out;
            m2._maximumItemsPerSecond = 1000;

            Machine m3 = CreateMachine(1, 0);
            m3._maximumItemsPerSecond = 4;
            m3._storageCapacity = 1000;
            m3._storageMode = Machine.StorageModes.In;

            MachineManager.ConnectMachines(m1, 0, m2, 0);
            MachineManager.ConnectMachines(m2, 0, m3, 0);
            
            Assert.AreEqual(m1._itemsPerSecondToOutputs, 3);
            Assert.AreEqual(m1._itemsPerSecondToStorage, 0);
            
            Assert.AreEqual(m2._itemsPerSecondToOutputs, 4);
            Assert.AreEqual(m2._itemsPerSecondToStorage, -1);
            
            Assert.AreEqual(m3._itemsPerSecondToOutputs, 0);
            Assert.AreEqual(m3._itemsPerSecondToStorage, 4);
        }

        [TestMethod()]
        public void UpdateDesiredProductionSpeedTestThreeMachinesWithMiddleStorageOutputDisabled()
        {
            Reset();
            List<Machine> machines = new List<Machine>();

            Machine m1 = CreateMachine(0, 1);
            m1._maximumItemsPerSecond = 5;

            Machine m2 = CreateMachine(1, 1);
            m2._maximumItemsPerSecond = 3;
            m2._storageCapacity = 1000;
            m2._itemsInStorage = 1000;
            m2._maximumItemsPerSecond = 1000;

            Machine m3 = CreateMachine(1, 0);
            m3._maximumItemsPerSecond = 4;
            m3._storageCapacity = 1000;
            m3._storageMode = Machine.StorageModes.In;

            MachineManager.ConnectMachines(m1, 0, m2, 0);
            MachineManager.ConnectMachines(m2, 0, m3, 0);
            
            Assert.AreEqual(m1._itemsPerSecondToOutputs, 3);
            Assert.AreEqual(m1._itemsPerSecondToStorage, 0);
            
            Assert.AreEqual(m2._itemsPerSecondToOutputs, 3);
            Assert.AreEqual(m2._itemsPerSecondToStorage, 0);
            
            Assert.AreEqual(m3._itemsPerSecondToOutputs, 0);
            Assert.AreEqual(m3._itemsPerSecondToStorage, 3);
        }

        [TestMethod()]
        public void UpdateDesiredProductionSpeedTestUnconnectedOutput()
        {
            Reset();
            List<Machine> machines = new List<Machine>();

            Machine m1 = CreateMachine(0, 1);
            m1._maximumItemsPerSecond = 3;

            Machine m2 = CreateMachine(1, 1);
            m2._maximumItemsPerSecond = 5;
            m2._storageCapacity = 1000;
            m2._storageMode = Machine.StorageModes.In;

            MachineManager.ConnectMachines(m1, 0, m2, 0);
            
            Assert.AreEqual(m1._itemsPerSecondToOutputs, 0);
            Assert.AreEqual(m1._itemsPerSecondToStorage, 0);
            
            Assert.AreEqual(m2._itemsPerSecondToOutputs, 0);
            Assert.AreEqual(m2._itemsPerSecondToStorage, 0);
        }

        [TestMethod()]
        public void UpdateDesiredProductionSpeedTestUnconnectedInput()
        {
            Reset();
            List<Machine> machines = new List<Machine>();

            Machine m1 = CreateMachine(1, 1);
            m1._maximumItemsPerSecond = 3;

            Machine m2 = CreateMachine(1, 0);
            m2._maximumItemsPerSecond = 5;
            m2._storageCapacity = 1000;
            m2._storageMode = Machine.StorageModes.In;

            MachineManager.ConnectMachines(m1, 0, m2, 0);
            
            Assert.AreEqual(m1._itemsPerSecondToOutputs, 0);
            Assert.AreEqual(m1._itemsPerSecondToStorage, 0);
            
            Assert.AreEqual(m2._itemsPerSecondToOutputs, 0);
            Assert.AreEqual(m2._itemsPerSecondToStorage, 0);
        }
    }
}