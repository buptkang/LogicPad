   M S B A M L   `   `   `  ����  GatesWpf    GatesWpf.SaveClose   5   	   \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      8x,http://schemas.microsoft.com/winfx/2006/xaml    5        � Title$  Save Changes?��5      $	��150��6   $	��400��6(    � 
ResizeMode$ NoResize=�64    � Icon$ Images/ram.png��6J   .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5	      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $��Images/Hard_drive.png��6   $��2��6/   $��15,15,15,15q�6@   �� 5   
   -   5        $��
lblCircuit��5      $��1��6&   $
��Wrap=�66   $��Arial)�6J   $
��Bold"�6]   $��	0,15,15,0q�6o   ><Do you want to save the changes you made in the circuit {0}?6�   �� 5   
   $��1��6   $��1��6$   $
��Wrap=�61   $��Arial)�6E   $��	0,10,15,0q�6X   31Your changes will be lost if you don't save them.6�   � 5   
   $��1��6   $��2��6    5      �� 5      $��75 �6#   �� 5      $��* �6#   �� 5      $��75 �6#   �� 5      $��* �6#   �� 5      $��50 �6#   �� 5      $��* �6#   .+�5      �� -   5        $��btnSave��5   %   $��0��6    �� 	IsDefault . .��6[   _Save6b   �� 5      -   5        $��btnDontSave��5   %   $��2��6   .��6R   _Don't Save6_   �� 5      -   5        $��	btnCancel��5   %   $��4��6    �� IsCancel . .��6^   	_Cancel6g   5      5       5!      