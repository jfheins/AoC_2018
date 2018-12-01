 run input.m
%input = [-6, +3, +8, +5, -6]';
sum(input);

inputRepeated = [0; repmat(input, [143 1])];

cumulated = cumsum(inputRepeated);

[uniq, idx, i] = unique(cumulated);

if (size(cumulated) == size(uniq))
  printf('No luck\r\n');
else
  duplicate_ind = setdiff(1:size(cumulated, 1), idx);
  duplicate_value = cumulated(duplicate_ind);
  
  
  for dupIdx = 1:size(duplicate_value, 1)
    equality = cumulated == duplicate_value(dupIdx);
    indexes = find(equality, 2);
    dupPosition(dupIdx) = indexes(2);
  endfor
  
  dupPositon = min(dupPosition);
  printf('First dup: %d\r\n', cumulated(dupPositon));
endif