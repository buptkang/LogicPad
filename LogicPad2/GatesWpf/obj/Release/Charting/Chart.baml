   M S B A M L   `   `   `  ���� J  FGatesWpf, Version=1.3.5887.26087, Culture=neutral, PublicKeyToken=null5clr-namespace:GatesWpf.DragDropGatesWpf.DragDrop  5clr-namespace:GatesWpf.ChartingGatesWpf.Charting      GatesWpf.Charting.Chart   \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    8x,http://schemas.microsoft.com/winfx/2006/xaml    2dragDropclr-namespace:GatesWpf.DragDrop    /localclr-namespace:GatesWpf.Charting      � Title$  Oscilloscope�� � Icon$ ../Images/LegendHS.png��$	��300��$	��400��.��� 
������� � ColumnDefinitions �� $��80 ��� $��* � � RowDefinitions �� $
��Auto ��� $��* ��� $
��Auto �.+��� -   $��slZoom�� � Minimum$ 0.001Q� � Maximum$ 10Q� � Value$ 1Q�$��1�� � ToolTip$
 Zoom��$��5,5,5,5q���  �� Orientation$ 
Horizontal=�$��5,5,5,5q�.+�d� -   $��btnPause��$��25��$ Pause��.���� $��../Images/PauseHS.png��$��20���� -   $��btnReset��$��25��$ Reset Display��.���� $$��../Images/RefreshDocViewHS.png��$��18���� -   $��lbLineLabelsScroller��$��1��$��0��$��Hidden=�.��*   $GatesWpf.DragDrop.ReorderableListBox  -   	  Name$	 lbLineLabels��*   $GatesWpf.DragDrop.ReorderableListBox
   ItemsReordered$
 lbLines_ItemsReordered��
�������$��0q��� -   $��lbLinesScroller��$��1��$��1��$��Visible=�.��  -   $	 lbLines��$
 lbLines_ItemsReordered��$��1��$��1��
�������!   GatesWpf.Charting.TickRuler  -     Name$ 	tickRuler��$��2��$��1��$��5,0,0,0q�