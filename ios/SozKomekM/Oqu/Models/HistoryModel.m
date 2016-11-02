//
//  HistoryModel.m
//  Oqu
//
//  Created by Ельнар Шопанов on 03.03.16.
//  Copyright © 2016 Ельнар Шопанов. All rights reserved.
//

#import "HistoryModel.h"
#import "Constants.h"

@implementation HistoryModel

@synthesize name = _name;
@synthesize wordId = _wordId;

+ (nonnull HistoryModel*)historyWithWordId:(NSInteger)wordId name:(nonnull NSString*)name {
    return [[HistoryModel alloc] initWithWordId:wordId name:name];
}

- (nonnull id)initWithWordId:(NSInteger)wordId name:(nonnull NSString*)name {
    if ((self = [super init]) != nil) {
        self.wordId = wordId;
        self.name = name;
    }
    
    return self;
}

- (id)initWithCoder:(nonnull NSCoder*)decoder {
    if ((self = [super init]) != nil) {
        self.wordId = [decoder decodeIntegerForKey:KEY_WORD_ID];
        self.name = [decoder decodeObjectForKey:KEY_NAME];
    }
    
    return self;
}

- (void)encodeWithCoder:(nonnull NSCoder *)encoder {
    [encoder encodeInteger:self.wordId forKey:KEY_WORD_ID];
    [encoder encodeObject:self.name forKey:KEY_NAME];
}

@end