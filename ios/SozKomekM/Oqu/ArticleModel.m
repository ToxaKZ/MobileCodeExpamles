//
//  ArticleModel.m
//  Oqu
//
//  Created by Ельнар Шопанов on 28.02.16.
//  Copyright © 2016 Ельнар Шопанов. All rights reserved.
//

#import "ArticleModel.h"

@implementation ArticleModel

@synthesize id = _id;
@synthesize name = _name;
@synthesize dicName = _dicName;
@synthesize fromCulture = _fromCulture;
@synthesize toCulture = _toCulture;

- (id)initWithId:(int)uniqueId name:(NSString *)name dicName:(NSString *)dicName fromCulture:(NSString *)fromCulture toCulture:(NSString *)toCulture {
    if ((self = [super init])) {
        self.id = uniqueId;
        self.name = name;
        self.dicName = dicName;
        self.fromCulture = fromCulture;
        self.toCulture = toCulture;
    }
    
    return self;
}

@end