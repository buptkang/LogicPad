   M S B A M L   `   `   `  ����J  FGatesWpf, Version=1.3.4526.23698, Culture=neutral, PublicKeyToken=null5clr-namespace:GatesWpf.DragDropGatesWpf.DragDrop  5   	   5clr-namespace:GatesWpf.ChartingGatesWpf.Charting  5   	       GatesWpf.Charting.Chart   5   	   \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      8x,http://schemas.microsoft.com/winfx/2006/xaml    5      2dragDropclr-namespace:GatesWpf.DragDrop    5   	   /localclr-namespace:GatesWpf.Charting    5   	     � Title$  Oscilloscope��5       � Icon$ ../Images/LegendHS.png��6   $	��300��68   $	��400��6E   .��5      � 
������� � ColumnDefinitions 5	   
   �� 5
      $��80 �6   �� 5      $��* �6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� -   5        $��slZoom��5   H    � Minimum$ 0.001Q�6    � Maximum$ 10Q�6!    � Value$ 1Q�6.   $��1��68    � ToolTip$
 Zoom��6y   $��5,5,5,5q�6�   �� 5   
    �� Orientation$ 
Horizontal=�6   $��5,5,5,5q�6.   .+�5      d� -   5        $��btnPause��5      $��25��6+   $ Pause��6n   .��5      �� $��../Images/PauseHS.png��6   $��20��67   5      �� 5      -   5        $��btnReset��5      $��25��6%   $ Reset Display��60   .��5      �� $$��../Images/RefreshDocViewHS.png��6   $��18��6@   5      5      �� 5   
   -   5        $��lbLineLabelsScroller��5      $��1��63   $��0��6@   $��Hidden=�6P   .��5      *   $GatesWpf.DragDrop.ReorderableListBox  -   5        	  Name$	 lbLineLabels��5   A   *   $GatesWpf.DragDrop.ReorderableListBox
   ItemsReordered$
 lbLines_ItemsReordered��6W   
�������$��0q�6   5      �� 5   
   -   5        $��lbLinesScroller��5      $��1��6/   $��1��6<   $��Visible=�6r   .��5         -   5        $	 lbLines��5    ^   $
 lbLines_ItemsReordered��6o   $��1��6*   $��1��67   
�������6o   5!      !   GatesWpf.Charting.TickRuler  5"   
   -   5          Name$ 	tickRuler��5"      $��2��6-   $��1��6:   $��5,0,0,0q�6J   5$      5&      