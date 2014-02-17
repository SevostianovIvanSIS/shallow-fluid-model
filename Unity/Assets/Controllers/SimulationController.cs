﻿using System.Threading;
using Engine.Geometry;
using Engine.Simulation;
using UnityEngine;

namespace Assets.Controllers
{
    public class SimulationController
    {
        public PrognosticFields CurrentFields 
        { 
            get { return _currentFieldsCache; } 
            set { _stepper.CurrentFields = value; }
        }
        private PrognosticFields _currentFieldsCache;

        public int NumberOfSteps { get; private set; }

        private readonly Thread _simulationThread;
        private readonly ManualResetEvent _pauseEvent;
        private readonly SimulationStepper _stepper;

        public SimulationController(IPolyhedron surface, ISimulationOptions options)
        {
            _stepper = new SimulationStepper(surface, options);
            _currentFieldsCache = _stepper.CurrentFields;

            _pauseEvent = new ManualResetEvent(false);
            _simulationThread = new Thread(SimulationLoop);
            _simulationThread.Start();
        }

        private void SimulationLoop()
        {
            while (true)
            {
                _pauseEvent.WaitOne();
                _stepper.StepSimulation();
                _currentFieldsCache = _stepper.CurrentFields;

                NumberOfSteps = NumberOfSteps + 1;
            }
        }


        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                TogglePause();
            }
        }

        public void TogglePause()
        {
            if (_pauseEvent.WaitOne(0))
            {
                _pauseEvent.Reset();
            }
            else
            {
                _pauseEvent.Set();
            }
        }


        public void Terminate()
        {
            _pauseEvent.Reset();
            _simulationThread.Abort();
        }
    }
}