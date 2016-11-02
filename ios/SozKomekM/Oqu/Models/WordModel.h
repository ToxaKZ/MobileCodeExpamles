//
//  WordModel.h
//  Oqu
//
//  Created by Ельнар Шопанов on 27.02.16.
//  Copyright © 2016 Ельнар Шопанов. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface WordModel : NSObject {
    int _id;
    NSString *_name;
}

@property (nonatomic, assign) int id;
@property (nonatomic, copy) NSString *name;

- (id)initWithId:(int)uniqueId name:(NSString*)name;

@end