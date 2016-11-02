//
//  WordModel.m
//  Oqu
//
//  Created by Ельнар Шопанов on 27.02.16.
//  Copyright © 2016 Ельнар Шопанов. All rights reserved.
//

#import "WordModel.h"

@implementation WordModel

@synthesize id = _id;
@synthesize name = _name;

- (id)initWithId:(int)uniqueId name:(NSString*)name {
    if ((self = [super init])) {
        self.id = uniqueId;
        self.name = name;
    }
    
    return self;
}

@end