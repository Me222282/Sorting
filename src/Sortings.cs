using System;
using System.Collections.Generic;

namespace Sorting
{
    public static class Sort
    {
        public static void BubbleSort(this BarContainer.ChildManager array, bool greater)
        {
            // Number of interations in pass of the sorting algorithm
            int interationsInPass = array.Length - 1;
            
            // Determins wether the array is sorted
            bool continuePass = true;
            // Keep calling passes until array is sorted
            while (continuePass)
            {
                // If no swaps were made, array is sorted
                continuePass = false;
                
                // Iterate through array
                for (int i = 0; i < interationsInPass; i++)
                {
                    // determin wether left is greater than, less than, or equal to right
                    bool swap = array[i].Value > array[i + 1].Value;
                    if (greater)
                    {
                        swap = !swap;
                    }
                    
                    // left is greater than right - section is sorted
                    if (!swap || (array[i] == array[i + 1])) { continue; }
                    
                    array.Swap(i, i + 1);
                    // Swap was made - array isn't sorted
                    continuePass = true;
                }
                
                // Final number is sorted, don't need to check again
                interationsInPass--;
            }
        }
        
        public static void InsertionSort(this BarContainer.ChildManager array, bool greater)
        {
            // Pass every index except 0
            for (int pass = 1; pass < array.Length; pass++)
            {
                for (int i = pass - 1; i >= 0; i--)
                {
                    // determin wether left is greater than, less than, or equal to right
                    bool swap = array[i].Value > array[i + 1].Value;
                    if (greater)
                    {
                        swap = !swap;
                    }
                    
                    // left is greater than right - section is sorted
                    if (!swap || (array[i] == array[i + 1])) { break; }
                    
                    // pass needs
                    array.Swap(i, i + 1);
                }
            }
        }
    }
}