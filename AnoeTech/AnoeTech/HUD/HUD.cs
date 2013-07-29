using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace AnoeTech
{
    public class HUD
    {
        private        bool _deactivating;
        private HUDObject[] _HUDObjects;
        private        bool _isActive, _isVisible;

        public int miniMapID;

        public enum HUDModes { AIRCRAFT, FREEROAM, INFANTRY, RTS, VEHICLE, WATERCRAFT };

        #region Accessors

        public     bool IsActive { get { return _isActive; }  set { _isActive = value; } }
        public    bool IsVisible { get { return _isVisible; } set { _isVisible = value; } }
        public HUDObject MiniMap { get { return _HUDObjects[miniMapID]; } }

        #endregion

        #region Functions

        public      void Activate()                                              // Activate the HUD and its elements
        {
            foreach (HUDObject hudObject in _HUDObjects)                    // For each widget
                if (hudObject != null)                                      // If it exists
                    hudObject.Activate();                                   // Activate the widget
            IsActive = _isVisible = true;                                   // Set the HUD to active and visible
        }

        public       int Add(HUDObject widget)                                   // Add a widget to the HUD
        {
            int flag = _HUDObjects.Length;                                  // Create a flag to record the first available slot
            for (int tmp = 0; tmp < _HUDObjects.Length; tmp++)              // Go through the array
                if (_HUDObjects[tmp] == null)                               // If an element is empty
                {
                    _HUDObjects[tmp] = widget;                              // Add the new widget in that element
                    return tmp;                                             // Return the widgets ID
                }

                                                                            // If the array is full
            HUDObject[] temp = new HUDObject[_HUDObjects.Length + 1];       // Make a new array with space for the new HUDObject

            for (int counter = 0; counter < _HUDObjects.Length; counter++)  // Copy over all the current HUD objects into the new array
                temp[counter] = _HUDObjects[counter];

            temp[_HUDObjects.Length] = widget;                              // Put the new object in the last spot of the array
            _HUDObjects = temp;                                             // Pass the new array of objects to the HUD
            return _HUDObjects.Length - 1;                                  // Return the location of the new HUDobject in the array
        }

        public      void Deactivate()                                            // Deactivate the HUD
        {
            foreach (HUDObject hudObject in _HUDObjects)                    // For each widget
                if (hudObject != null)                                         // If the widget exists
                    hudObject.Deactivate();                                 // Deactivate the widget
            _deactivating = true;                                           // Set Hud is deactivating flag to true
        }

        public      void Draw()                                                  // Draw all widgets
        {
            foreach (HUDObject hudObject in _HUDObjects)                    // For each widget
                if (hudObject != null)                                       // If it exists
                    if (hudObject.IsVisible)                               // And is visible
                        hudObject.Draw();                                   // Then Draw it
        }

        public HUDObject GetObject(int ID)                                       // Get a reference to a widget
        {
            return _HUDObjects[ID];                                         // Return the widget with given ID
        }

        public      void Initiate()                                // Initiate the HUD
        {
            _HUDObjects = new HUDObject[0];                                 // Create a new array
            _isVisible = _isActive = false;                                 // Set active and visible to false
        }

        public      void Remove(int ID)                                          // Remove a widget from the HUD
        {
            _HUDObjects[ID] = null;                                         // Set the HUD object to null
        }                                     

        public      void Update()                                                // Update the HUD
        {
            foreach (HUDObject hudObject in _HUDObjects)                    // Update all widgets
                if(hudObject!=null)                                         // If the widget exists
                    hudObject.Update( null );                               // Update it with null parameter

            if (_deactivating)                                              // If the HUD is deactivating
            {
                bool allWidgetsDeactivated = true;                          // All widgets are done deactivating
                for (int tmp = 0; tmp < _HUDObjects.Length; tmp++)          // Loop through each widget
                {
                    if (_HUDObjects[tmp] != null)                           // If the widget exists
                        if (_HUDObjects[tmp].IsVisible)                     // and is visible
                            allWidgetsDeactivated = false;                  // Then set allWidgetsDeactivated to false
                }
                if (allWidgetsDeactivated) _isVisible = _isActive = false;  // If all the widgets were deactivated then set the HUD to dactivated
            }
        }

        #endregion
    }
}
