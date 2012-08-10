#region License
/*
Copyright (c) 2005-2012, CellAO Team

All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion

#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace AO.Core
{
    /// <summary>
    /// Core Inventory Class
    /// </summary>
    public class AOInventory
    {
        /// <summary>
        /// List of Inventory pages (Weaponpage, Armorpage etc)
        /// </summary>
        public List<AOInventoryPage> Pages = new List<AOInventoryPage>();

        /// <summary>
        /// Returns InventoryEntry in container at place
        /// </summary>
        /// <param name="container">Container number</param>
        /// <param name="place">Placement</param>
        /// <returns>InventoryEntry or null (not found)</returns>
        public InventoryEntry GetInventoryEntryAt(int container, int place)
        {
            foreach (AOInventoryPage page in Pages)
            {
                if (page.Container == container)
                {
                    foreach (InventoryEntry iv in page.Entries)
                    {
                        if (iv.Placement == place)
                        {
                            return iv;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a InventoryPage
        /// </summary>
        /// <param name="num">Container number</param>
        /// <returns>InventoryPage or null (not found)</returns>
        public AOInventoryPage GetPage(int num)
        {
            foreach (AOInventoryPage page in Pages)
            {
                if (page.Container == num)
                    return page;
            }
            return null;
        }

        /// <summary>
        /// Add item to main inventory
        /// </summary>
        /// <param name="it">AOItem to add to inventory</param>
        /// <returns>Resulting InventoryEntry</returns>
        public InventoryEntry AddItem(AOItem it)
        {
            return AddItem(0x6f, 0, it);
        }


        /// <summary>
        /// Add a Item to a Inventory page/place
        /// </summary>
        /// <param name="container">Number of Inventory page</param>
        /// <param name="place">Desired place</param>
        /// <param name="item">item to add</param>
        /// <returns>Success</returns>
        public InventoryEntry AddItem(int container, int place, AOItem item)
        {
            // Container ID's:
            // (x) 0065 Weaponpage 
            // (x) 0066 Armorpage
            // (x) 0067 Implantpage
            // (x) 0068 Inventory (places 64-93)
            // (x) 0069 Bank
            // ( ) 006B Backpack - this will take some time
            // ( ) 006C (KnuBot) Trade Window
            // ( ) 006E Overflow window
            // (x) 006F Trade Window/Next free spot in 0x68
            // (x) 0073 Socialpage
            // ( ) 0767 Shop Inventory
            // ( ) 0790 Playershop Inventory
            // (x) DEAD Bank (why FC, why???)

            switch (container)
            {
                // Equipment pages
                case 0x65:
                case 0x66:
                case 0x67:
                case 0x73:
                    {
                        if (GetInventoryEntryAt(container, place) == null)
                        {
                            InventoryEntry newentry = new InventoryEntry();
                            newentry.Item = item.ShallowCopy();
                            newentry.Placement = place;

                            GetPage(container).Entries.Add(newentry);
                            return newentry;
                        }
                        return null;
                    }
                // Look for next free main inventory spot
                case 0x6f:
                    {
                        int nextfree = 64;
                        while (nextfree < 94)
                        {
                            if (GetInventoryEntryAt(0x68, nextfree) == null)
                            {
                                InventoryEntry newentry = new InventoryEntry();
                                newentry.Item = item.ShallowCopy();
                                newentry.Placement = nextfree;

                                GetPage(0x68).Entries.Add(newentry);
                                return newentry;
                            }
                            nextfree++;
                        }
                        return null;
                    }
                // Bank
                case 0xDEAD:
                case 0x69: // 0x69 probably not needed
                    {
                        container = 0x69;
                        int nextfree = 0;
                        while (nextfree < 102)
                        {
                            if (GetInventoryEntryAt(container, nextfree) == null)
                            {
                                InventoryEntry newentry = new InventoryEntry();
                                newentry.Item = item.ShallowCopy();
                                newentry.Placement = nextfree;

                                GetPage(container).Entries.Add(newentry);
                                return newentry;
                            }
                            nextfree++;
                        }
                        return null;
                    }

            }
            return null;
        }

        /// <summary>
        /// Remove a item from inventory
        /// </summary>
        /// <param name="container">Container</param>
        /// <param name="place">Place</param>
        public void Remove(int container, int place)
        {
            GetPage(container).Entries.Remove(GetInventoryEntryAt(container, place));
        }
    }

    /// <summary>
    /// Inventory Page class
    /// </summary>
    public class AOInventoryPage
    {
        /// <summary>
        /// List of InventoryEntries
        /// </summary>
        public List<InventoryEntry> Entries = new List<InventoryEntry>();

        /// <summary>
        /// Container number
        /// </summary>
        public int Container = -1;
        int Low = -1;
        int Spots = -1;

        /// <summary>
        /// Creates a inventory page
        /// </summary>
        /// <param name="container">Container number</param>
        /// <param name="min">Starting index</param>
        /// <param name="numberofspots">Number of spots in this page</param>
        public AOInventoryPage(int container, int min, int numberofspots)
        {
            Container = container;
            Low = min;
            Spots = numberofspots;
        }

        public void AddItem(AOItem item)
        {
            int nextfree = Low;
            bool free = false;
            while (nextfree < Low + Spots)
            {
                free = true;
                foreach (InventoryEntry ie in Entries)
                {
                    if (nextfree == ie.Placement)
                    {
                        free = false;
                        break;
                    }
                }
                if (free)
                    break;

                nextfree++;
            }
            if (free)
            {
                InventoryEntry newentry = new InventoryEntry();
                newentry.Placement = nextfree;
                newentry.Item = item.ShallowCopy();
                Entries.Add(newentry);
            }
        }

    }

    /// <summary>
    /// Inventory Entry, holding Item and placement inside its container
    /// </summary>
    public class InventoryEntry
    {
        /// <summary>
        /// Placement number
        /// </summary>
        public int Placement;

        /// <summary>
        /// Item
        /// </summary>
        public AOItem Item = new AOItem();
    }

}
